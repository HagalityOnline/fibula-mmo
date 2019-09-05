// -----------------------------------------------------------------
// <copyright file="VocationType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of possible vocation types.
    /// </summary>
    public enum VocationType : byte
    {
        /// <summary>
        /// No vocation yet.
        /// </summary>
        None = 0,

        /// <summary>
        /// The best at melee combat.
        /// </summary>
        Knight = 1,

        /// <summary>
        /// Sharpshooter, the best at ranged combat.
        /// </summary>
        Paladin = 2,

        /// <summary>
        /// Expert in magic, specialized in aggresive and damaging spells.
        /// </summary>
        Sorcerer = 3,

        /// <summary>
        /// Expert in magic, specialized in healing and nature spells.
        /// </summary>
        Druid = 4,
    }
}
