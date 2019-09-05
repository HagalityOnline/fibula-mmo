// -----------------------------------------------------------------
// <copyright file="IMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Structs;

    public interface IMap
    {
        ITile this[Location location] { get; }

        ITile this[ushort x, ushort y, sbyte z] { get; }

        ConcurrentDictionary<Location, ITile> Tiles { get; }

        IList<byte> GetDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);

        IList<byte> GetFloorDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX, byte windowSizeY, int verticalOffset, ref int skip);

        IList<byte> GetTileDescription(IPlayer player, ITile tile);

        void Load(int fromX = 0, int toX = 0, int fromY = 0, int toY = 0, byte fromZ = 0, byte toZ = 0);

        void Load(Location atLocation);
    }
}