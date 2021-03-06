﻿// -----------------------------------------------------------------
// <copyright file="BloodType.cs" company="2Dudes">
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
    /// Enumerates the possible blood types.
    /// </summary>
    public enum BloodType : byte
    {
        /// <summary>
        /// Normal blood.
        /// </summary>
        Blood,

        /// <summary>
        /// Fire.
        /// </summary>
        Fire,

        /// <summary>
        /// Slime.
        /// </summary>
        Slime,

        /// <summary>
        /// Bones.
        /// </summary>
        Bones,
    }
}
