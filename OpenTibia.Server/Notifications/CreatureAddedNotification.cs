// <copyright file="CreatureAddedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for creature being added in sight to players who are close.
    /// </summary>
    internal class CreatureAddedNotification : ProximityNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureAddedNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureAddedNotification(CreatureAddedNotificationArguments arguments)
            : base(arguments?.Location ?? default)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureAddedNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public override void Prepare()
        {
            if (!(Game.Instance.GetCreatureWithId(this.PlayerId) is IPlayer player))
            {
                return;
            }

            if (this.Arguments.AddedEffect != AnimatedEffect.None)
            {
                this.Packets.Add(new MagicEffectPacket(this.Arguments.Creature.Location, this.Arguments.AddedEffect));
            }

            this.Packets.Add(new AddCreaturePacket(this.Arguments.Creature, player.KnowsCreatureWithId(this.Arguments.Creature.Id), player.ChooseToRemoveFromKnownSet()));
        }
    }
}