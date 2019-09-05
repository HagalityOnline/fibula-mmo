﻿// <copyright file="ICombatant.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using OpenTibia.Server.Contracts.Enumerations;

    public interface ICombatant : ICreature, INeedsCooldowns
    {
        event OnAttackTargetChange OnTargetChanged;

        /// <summary>
        /// Gets the id of the actor.
        /// </summary>
        uint ActorId { get; }

        /// <summary>
        /// Gets the blood type of the actor.
        /// </summary>
        BloodType Blood { get; }

        uint AutoAttackTargetId { get; }

        byte AutoAttackRange { get; }

        byte AutoAttackCredits { get; }

        byte AutoDefenseCredits { get; }

        DateTimeOffset LastAttackTime { get; }

        TimeSpan LastAttackCost { get; }

        TimeSpan CombatCooldownTimeRemaining { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoAttack credit per second.
        /// </summary>
        decimal BaseAttackSpeed { get; }

        /// <summary>
        /// Gets a metric of how fast an Actor can earn a new AutoDefense credit per second.
        /// </summary>
        decimal BaseDefenseSpeed { get; }

        ushort AttackPower { get; }

        ushort DefensePower { get; }

        ushort ArmorRating { get; }

        void SetAttackTarget(uint targetId);

        void UpdateLastAttack(TimeSpan cost);

        void CheckAutoAttack(IThing thingChanged, ThingStateChangedEventArgs eventAgrs);
    }
}