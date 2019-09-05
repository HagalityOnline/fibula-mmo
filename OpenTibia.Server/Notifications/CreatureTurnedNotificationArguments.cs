// <copyright file="CreatureTurnedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class CreatureTurnedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="turnEffect"></param>
        public CreatureTurnedNotificationArguments(ICreature creature, AnimatedEffect turnEffect = AnimatedEffect.None)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
            this.TurnedEffect = turnEffect;
        }

        public ICreature Creature { get; }

        public AnimatedEffect TurnedEffect { get; }
    }
}