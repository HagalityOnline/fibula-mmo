﻿// -----------------------------------------------------------------
// <copyright file="CreatureType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates the different types of creatures.
    /// </summary>
    public enum CreatureType
    {
        /// <summary>
        /// An NPC.
        /// </summary>
        NonPlayerCharacter,

        /// <summary>
        /// A player.
        /// </summary>
        Player,

        /// <summary>
        /// A monster.
        /// </summary>
        Monster,
    }
}
