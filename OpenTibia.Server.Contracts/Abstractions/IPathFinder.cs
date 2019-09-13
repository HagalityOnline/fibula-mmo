﻿// -----------------------------------------------------------------
// <copyright file="IPathFinder.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for a path finder.
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Attempts to find a path between two <see cref="Location"/>s.
        /// </summary>
        /// <param name="startLocation">The start location.</param>
        /// <param name="targetLocation">The target location to find a path to.</param>
        /// <param name="endLocation">The last searched location before returning.</param>
        /// <param name="maxStepsCount">Optional. The maximum number of search steps to perform before giving up on finding the target location. Default is 100.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Direction"/>s leading to the end location. The <paramref name="endLocation"/> and <paramref name="targetLocation"/> may or may not be the same.</returns>
        IEnumerable<Direction> FindBetween(Location startLocation, Location targetLocation, out Location endLocation, int maxStepsCount = 100);
    }
}