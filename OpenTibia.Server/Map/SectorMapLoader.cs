// <copyright file="SectorMapLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Map
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Parsing;

    public class SectorMapLoader : IMapLoader
    {
        // TODO: to configuration
        private static readonly Lazy<ConnectionMultiplexer> CacheConnectionInstance = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("<redis connection string>"));

        public static ConnectionMultiplexer CacheConnection => CacheConnectionInstance.Value;

        public const char CommentSymbol = '#';
        public const char SectorSeparator = ':';
        public const char PositionSeparator = '-';

        public const string AttributeSeparator = ",";
        public const string AttributeDefinition = "=";

        public const int SectorXMin = 996;
        public const int SectorXMax = 1043;

        public const int SectorYMin = 984;
        public const int SectorYMax = 1031;

        public const int SectorZMin = 0;
        public const int SectorZMax = 15;

        private readonly DirectoryInfo mapDirInfo;
        private readonly bool[,,] sectorsLoaded;

        private long totalTileCount;
        private long totalLoadedCount;

        public byte PercentageComplete => (byte)Math.Floor(Math.Min(100, Math.Max(0M, this.totalLoadedCount * 100 / (this.totalTileCount + 1))));

        /// <summary>
        /// Initializes a new instance of the <see cref="SectorMapLoader"/> class.
        /// </summary>
        /// <param name="creatureFinder"></param>
        /// <param name="mapFilesPath"></param>
        public SectorMapLoader(ICreatureFinder creatureFinder, string mapFilesPath)
        {
            mapFilesPath.ThrowIfNullOrWhiteSpace(nameof(mapFilesPath));

            this.mapDirInfo = new DirectoryInfo(mapFilesPath);

            this.CreatureFinder = creatureFinder;

            this.totalTileCount = 1;
            this.totalLoadedCount = default;
            this.sectorsLoaded = new bool[SectorXMax - SectorXMin, SectorYMax - SectorYMin, SectorZMax - SectorZMin];
        }

        public ICreatureFinder CreatureFinder { get; }

        // public ITile[,,] LoadFullMap()
        // {
        //    return Load(SectorXMin, SectorXMax, SectorYMin, SectorYMax, SectorZMin, SectorZMax);
        // }

        public ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ)
        {
            if (toSectorX < fromSectorX || toSectorY < fromSectorY || toSectorZ < fromSectorZ)
            {
                throw new InvalidOperationException("Bad range supplied.");
            }

            var tiles = new ITile[(toSectorX - fromSectorX + 1) * 32, (toSectorY - fromSectorY + 1) * 32, toSectorZ - fromSectorZ + 1];

            this.totalTileCount = tiles.LongLength;
            this.totalLoadedCount = default;

            IDatabase cache = CacheConnection.GetDatabase();

            Parallel.For(fromSectorZ, toSectorZ + 1, sectorZ =>
            {
                Parallel.For(fromSectorY, toSectorY + 1, sectorY =>
                {
                    Parallel.For(fromSectorX, toSectorX + 1, sectorX =>
                    {
                        var sectorFileName = $"{sectorX:0000}-{sectorY:0000}-{sectorZ:00}.sec";

                        string fileContents = cache.StringGet(sectorFileName);

                        if (fileContents == null)
                        {
                            var fullFilePath = Path.Combine(this.mapDirInfo.FullName, sectorFileName);
                            var sectorFileInfo = new FileInfo(fullFilePath);

                            if (sectorFileInfo.Exists)
                            {
                                using (var streamReader = sectorFileInfo.OpenText())
                                {
                                    fileContents = streamReader.ReadToEnd();

                                    cache.StringSet(sectorFileName, fileContents);
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(fileContents))
                        {
                            var loadedTiles = this.ReadSector(sectorFileName, fileContents, (ushort)(sectorX * 32), (ushort)(sectorY * 32), (sbyte)sectorZ);

                            Parallel.ForEach(loadedTiles, tile =>
                            {
                                tiles[tile.Location.X - (fromSectorX * 32), tile.Location.Y - (fromSectorY * 32), tile.Location.Z - fromSectorZ] = tile;
                            });
                        }

                        Interlocked.Add(ref this.totalLoadedCount, 1024); // 1024 per sector file, regardless if there is a tile or not...
                        this.sectorsLoaded[sectorX - SectorXMin, sectorY - SectorYMin, sectorZ - SectorZMin] = true;
                    });
                });
            });

            this.totalLoadedCount = this.totalTileCount;

            return tiles;
        }

        public bool HasLoaded(int x, int y, byte z)
        {
            if (x > SectorXMax)
            {
                return this.sectorsLoaded[(x / 32) - SectorXMin, (y / 32) - SectorYMin, z - SectorZMin];
            }

            return this.sectorsLoaded[x - SectorXMin, y - SectorYMin, z - SectorZMin];
        }

        public IList<Tile> ReadSector(string fileName, string sectorFileContents, ushort xOffset, ushort yOffset, sbyte z)
        {
            var loadedTilesList = new List<Tile>();

            var lines = sectorFileContents.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var readLine in lines)
            {
                var inLine = readLine?.Split(new[] { CommentSymbol }, 2).FirstOrDefault();

                // ignore comments and empty lines.
                if (string.IsNullOrWhiteSpace(inLine))
                {
                    continue;
                }

                var data = inLine.Split(new[] { SectorSeparator }, 2);

                if (data.Length != 2)
                {
                    throw new Exception($"Malformed line [{inLine}] in sector file: [{fileName}]");
                }

                var tileInfo = data[0].Split(new[] { PositionSeparator }, 2);
                var tileData = data[1];

                var newTile = new Tile(
                    this.CreatureFinder,
                    new Location
                    {
                        X = (ushort)(xOffset + Convert.ToUInt16(tileInfo[0])),
                        Y = (ushort)(yOffset + Convert.ToUInt16(tileInfo[1])),
                        Z = z,
                    });

                // load and add tile flags and contents.
                foreach (var element in CipParser.Parse(tileData))
                {
                    foreach (var attribute in element.Attributes)
                    {
                        if (attribute.Name.Equals("Content"))
                        {
                            newTile.AddParsedContent(attribute.Value);
                        }
                        else
                        {
                            // it's a flag
                            if (Enum.TryParse(attribute.Name, out TileFlag flagMatch))
                            {
                                newTile.SetFlag(flagMatch);
                            }
                            else
                            {
                                // TODO: proper logging.
                                Console.WriteLine($"Unknown flag [{attribute.Name}] found on tile at location {newTile.Location}.");
                            }
                        }
                    }
                }

                loadedTilesList.Add(newTile);
            }

            // TODO: proper logging.
            // Console.WriteLine($"Sector file {sectorFileContents.Name}: {loadedTilesList.Count} tiles loaded.");
            return loadedTilesList;
        }
    }
}
