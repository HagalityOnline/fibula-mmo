// -----------------------------------------------------------------
// <copyright file="TileFlag.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Contracts.Enumerations
{
    public enum TileFlag : byte
    {
        None = 0,

        Refresh = 1 << 0,

        ProtectionZone = 1 << 1,

        NoLogout = 1 << 2,
    }
}
