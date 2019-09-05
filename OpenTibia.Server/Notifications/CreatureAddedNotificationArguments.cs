// <copyright file="CreatureAddedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Internal class that represents arguments for a creature added notification.
    /// </summary>
    internal class CreatureAddedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureAddedNotificationArguments"/> class.
        /// </summary>
        /// <param name="location">The location at which the creature was added.</param>
        /// <param name="creature">The creature that was added.</param>
        /// <param name="addEffect">Optional. The effect to display.</param>
        public CreatureAddedNotificationArguments(Location location, ICreature creature, AnimatedEffect addEffect = AnimatedEffect.None)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Location = location;
            this.Creature = creature;
            this.AddedEffect = addEffect;
        }

        /// <summary>
        /// Gets the location of the creature added.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the creatue added.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the effect to display at the creature's location.
        /// </summary>
        public AnimatedEffect AddedEffect { get; }
    }
}