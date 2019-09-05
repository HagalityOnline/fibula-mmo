// -----------------------------------------------------------------
// <copyright file="CreatureFlag.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum CreatureFlag : uint
    {
        None = 0,
        KickBoxes = 1 >> 1,
        KickCreatures = 1 >> 2,
        SeeInvisible = 1 >> 3,
        Unpushable = 1 >> 4,
        DistanceFighting = 1 >> 5,
        NoSummon = 1 >> 6,
        NoIllusion = 1 >> 7,
        NoConvince = 1 >> 8,
        NoBurning = 1 >> 9,
        NoPoison = 1 >> 10,
        NoEnergy = 1 >> 11,
        NoParalyze = 1 >> 12,
        NoHit = 1 >> 13,
        NoLifeDrain = 1 >> 14,
    }
}
