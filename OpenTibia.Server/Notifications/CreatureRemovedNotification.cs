// <copyright file="CreatureRemovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;

    internal class CreatureRemovedNotification : ProximityNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureRemovedNotification(CreatureRemovedNotificationArguments arguments)
            : base(audience, playerId)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureRemovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(this.PlayerId);

            if (player == null || !player.CanSee(this.Arguments.Creature) || !player.CanSee(this.Arguments.Creature.Location))
            {
                return;
            }

            this.Packets.Add(new RemoveAtStackposPacket(this.Arguments.Creature.Location, this.Arguments.OldStackPosition));

            if (this.Arguments.RemoveEffect != AnimatedEffect.None)
            {
                this.Packets.Add(new MagicEffectPacket(this.Arguments.Creature.Location, AnimatedEffect.Puff));
            }
        }
    }
}