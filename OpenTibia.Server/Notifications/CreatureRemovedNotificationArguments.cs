// -----------------------------------------------------------------
// <copyright file="CreatureRemovedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class CreatureRemovedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="oldStackPos"></param>
        /// <param name="removeEffect"></param>
        public CreatureRemovedNotificationArguments(ICreature creature, byte oldStackPos, AnimatedEffect removeEffect = AnimatedEffect.None)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.OldStackPosition = oldStackPos;
            this.RemoveEffect = removeEffect;
        }

        public AnimatedEffect RemoveEffect { get; }

        public byte OldStackPosition { get; }

        public ICreature Creature { get; }
    }
}