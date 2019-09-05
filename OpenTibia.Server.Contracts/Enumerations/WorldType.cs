// -----------------------------------------------------------------
// <copyright file="WorldType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum WorldType : byte
    {
        /// <summary>
        /// No PvP allowed.
        /// </summary>
        Safe,

        /// <summary>
        /// PvP is allowed but punished.
        /// </summary>
        Normal,

        /// <summary>
        /// PvP is unpunished.
        /// </summary>
        Hardcore,
    }
}
