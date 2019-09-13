// -----------------------------------------------------------------
// <copyright file="IMap.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for a map.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// Gets the tile at a given <see cref="Location"/> in this map.
        /// </summary>
        /// <param name="location">The location to get the tile for.</param>
        /// <returns>A tile instance, if a tile exists at the location, null otherwise.</returns>
        ITile this[Location location] { get; }

        IEnumerable<byte> GetDescription(IPlayer asPlayer, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);

        IEnumerable<byte> GetFloorDescription(IPlayer asPlayer, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX, byte windowSizeY, int verticalOffset, ref int skip);

        IEnumerable<byte> GetTileDescription(IPlayer asPlayer, ITile tile);
    }
}