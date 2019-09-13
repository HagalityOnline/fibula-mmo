// <copyright file="CreatureRemovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;

    internal class CreatureRemovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureRemovedNotification"/> class.
        /// </summary>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureRemovedNotification(Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureRemovedNotificationArguments arguments)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureRemovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
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