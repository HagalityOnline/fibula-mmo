// <copyright file="ICreature.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public delegate void OnCreatureStateChange();

    public interface ICreature : IThing, INeedsCooldowns
    {
        // event OnCreatureStateChange OnZeroHealth;
        // event OnCreatureStateChange OnInventoryChanged;

        /// <summary>
        /// Gets the creature's id.
        /// </summary>
        Guid Id { get; }

        string Article { get; }

        string Name { get; }

        ushort Corpse { get; }

        ushort Hitpoints { get; }

        ushort MaxHitpoints { get; }

        ushort Manapoints { get; }

        ushort MaxManapoints { get; }

        decimal CarryStrength { get; }

        Outfit Outfit { get; }

        Direction Direction { get; }

        Direction ClientSafeDirection { get; }

        byte LightBrightness { get; }

        byte LightColor { get; }

        ushort Speed { get; }

        uint Flags { get; }

        ConcurrentQueue<Tuple<byte, Direction>> WalkingQueue { get; }

        byte NextStepId { get; set; }

        IDictionary<SkillType, ISkill> Skills { get; }

        bool IsInvisible { get; } // TODO: implement.

        bool CanSeeInvisible { get; } // TODO: implement.

        byte Skull { get; } // TODO: implement.

        byte Shield { get; } // TODO: implement.

        IInventory Inventory { get; }

        byte GetStackPosition();

        bool CanSee(ICreature creature);

        bool CanSee(Location location);

        void TurnToDirection(Direction direction);

        void StopWalking();

        void AutoWalk(params Direction[] directions);

        TimeSpan CalculateRemainingCooldownTime(ExhaustionType type, DateTimeOffset currentTime);

        void UpdateLastStepInfo(byte lastStepId, bool wasDiagonal = true);

        IEnumerable<(IEvent Event, TimeSpan Delay)> Think();
    }
}
