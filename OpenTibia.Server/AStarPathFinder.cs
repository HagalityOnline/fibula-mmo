// -----------------------------------------------------------------
// <copyright file="AStarPathFinder.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Algorithms;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Class that represents a path finder that implements the A* algorithm to find a path bewteen two <see cref="Location"/>s.
    /// </summary>
    public class AStarPathFinder : IPathFinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarPathFinder"/> class.
        /// </summary>
        /// <param name="tileAccessor">A refernce to the file accessor.</param>
        public AStarPathFinder(ITileAccessor tileAccessor)
        {
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            this.TileAccessor = tileAccessor;
        }

        /// <summary>
        /// Gets the tile accessor in use.
        /// </summary>
        public ITileAccessor TileAccessor { get; }

        /// <summary>
        /// Attempts to find a path using the <see cref="AStar"/> implementation between two <see cref="Location"/>s.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="targetLocation">The target location to find a path to.</param>
        /// <param name="endLocation">The last searched location before returning.</param>
        /// <param name="maxStepsCount">Optional. The maximum number of search steps to perform before giving up on finding the target location. Default is 100.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to the end location. The <paramref name="endLocation"/> and <paramref name="targetLocation"/> may or may not be the same.</returns>
        public IEnumerable<Direction> FindBetween(Location startLocation, Location targetLocation, out Location endLocation, int maxStepsCount = 100)
        {
            endLocation = startLocation;

            var fromTile = this.TileAccessor.GetTileAt(startLocation);
            var toTile = this.TileAccessor.GetTileAt(targetLocation);

            var searchId = Guid.NewGuid().ToString();
            var aSar = new AStar(TileNodeCache.Create(searchId, fromTile), TileNodeCache.Create(searchId, toTile), maxStepsCount);

            var result = aSar.Run();
            var dirList = new List<Direction>();

            try
            {
                if (result == SearchState.Failed)
                {
                    var lastTile = aSar.GetPath()?.LastOrDefault() as TileNode;

                    if (lastTile?.Tile != null)
                    {
                        endLocation = lastTile.Tile.Location;
                    }

                    return dirList;
                }

                var lastLoc = startLocation;

                foreach (var node in aSar.GetPath().Cast<TileNode>().Skip(1))
                {
                    var newDir = lastLoc.DirectionTo(node.Tile.Location, true);

                    dirList.Add(newDir);

                    lastLoc = node.Tile.Location;
                }

                endLocation = lastLoc;
            }
            finally
            {
                TileNodeCache.CleanUp(searchId);
            }

            return dirList;
        }
    }
}
