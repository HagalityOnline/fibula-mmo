// -----------------------------------------------------------------
// <copyright file="ProjectileType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum ProjectileType : byte
    {
        Spear = 0x00,
        Bolt = 0x01,
        Arrow = 0x02,
        Fire = 0x03,
        Energy = 0x04,
        PoisonArrow = 0x05,
        BurstArrow = 0x06,
        ThrowingStar = 0x07,
        ThrowingKnife = 0x08,
        SmallStone = 0x09,
        SuddenDeath = 0x0A,
        LargeRock = 0x0B,
        Snowball = 0x0C,
        PowerBolt = 0x0D,
        None = 0xFF, // Don't send to client.
    }
}
