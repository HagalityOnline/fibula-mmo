// -----------------------------------------------------------------
// <copyright file="Gender.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of different possible genders.
    /// </summary>
    public enum Gender : byte
    {
        /// <summary>
        /// Males.
        /// </summary>
        Male = 0x00,

        /// <summary>
        /// Females.
        /// </summary>
        Female = 0x01,
    }
}
