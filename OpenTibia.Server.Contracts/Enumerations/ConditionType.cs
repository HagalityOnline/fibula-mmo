// -----------------------------------------------------------------
// <copyright file="ConditionType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum ConditionType : uint
    {
        None = 0,
        Posion = 1 << 0,
        Fire = 1 << 1,
        Energy = 1 << 2,
        LifeDrain = 1 << 3,
        Haste = 1 << 4,
        Paralyze = 1 << 5,
        Outfit = 1 << 6,
        Invisible = 1 << 7,
        Light = 1 << 8,
        ManaShield = 1 << 9,
        InFight = 1 << 10,
        PzLocked = 1 << 11,
        Drunk = 1 << 12,
        Muted = 1 << 13,
        ExhaustCombat = 1 << 14,
        ExhaustHeal = 1 << 15,
        ExhaustYell = 1 << 16,
    }
}
