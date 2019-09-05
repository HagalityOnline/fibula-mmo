// <copyright file="IMonsterType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public interface IMonsterType
    {
        bool Locked { get; }

        ushort RaceId { get; }

        string Name { get; }

        string Article { get; }

        uint Experience { get; }

        ushort SummonCost { get; }

        ushort FleeThreshold { get; }

        byte LoseTarget { get; }

        ushort ConditionInfect { get; } // Holds ConditionType that this monster infects upon dealt damage.

        HashSet<KnownSpell> KnownSpells { get; }

        HashSet<CreatureFlag> Flags { get; }

        Dictionary<SkillType, ISkill> Skills { get; }

        List<string> Phrases { get; }

        List<Tuple<ushort, byte, ushort>> InventoryComposition { get; }

        (byte, byte, byte, byte) Strategy { get; }

        ushort Attack { get; }

        ushort Defense { get; }

        ushort Armor { get; }

        Outfit Outfit { get; }

        ushort Corpse { get; }

        ushort MaxHitPoints { get; }

        ushort MaxManaPoints { get; }

        BloodType Blood { get; }

        ushort Speed { get; }

        ushort Capacity { get; }
    }
}
