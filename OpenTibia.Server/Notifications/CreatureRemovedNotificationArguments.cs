﻿// -----------------------------------------------------------------
// <copyright file="CreatureRemovedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents arguments for the creature being removed notification.
    /// </summary>
    internal class CreatureRemovedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature">The creature being removed.</param>
        /// <param name="oldStackPos">The position in the stack of the creature being removed.</param>
        /// <param name="removeEffect">Optional. An effect to add when removing the creature.</param>
        public CreatureRemovedNotificationArguments(ICreature creature, byte oldStackPos, AnimatedEffect removeEffect = AnimatedEffect.None)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.OldStackPosition = oldStackPos;
            this.RemoveEffect = removeEffect;
        }

        /// <summary>
        /// Gets the effect to send when removing the creature.
        /// </summary>
        public AnimatedEffect RemoveEffect { get; }

        /// <summary>
        /// Gets the old stack position of the creature.
        /// </summary>
        public byte OldStackPosition { get; }

        /// <summary>
        /// Gets the creature being removed.
        /// </summary>
        public ICreature Creature { get; }
    }
}