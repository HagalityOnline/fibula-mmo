// <copyright file="IMapLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    /// <summary>
    /// Common interface for map loaders.
    /// </summary>
    public interface IMapLoader
    {
        /// <summary>
        /// Gets the percentage completed loading the map [0, 100].
        /// </summary>
        byte PercentageComplete { get; }

        /// <summary>
        /// Gets a value indicating whether this loader has previously loaded the given coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        /// <returns>True if the loader has previously loaded the given coordinates, false otherwise.</returns>
        bool HasLoaded(int x, int y, byte z);

        ITile[,,] Load(int fromSectorX, int toSectorX, int fromSectorY, int toSectorY, byte fromSectorZ, byte toSectorZ);
    }
}