// -----------------------------------------------------------------
// <copyright file="WorldState.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum WorldState : byte
    {
        /// <summary>
        /// Represents a world in loading state.
        /// </summary>
        Creating,

        /// <summary>
        /// The normal, open public state.
        /// </summary>
        Open,

        /// <summary>
        /// Testing or closed beta state.
        /// </summary>
        Closed,
    }
}
