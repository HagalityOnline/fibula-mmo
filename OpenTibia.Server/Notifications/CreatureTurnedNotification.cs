// <copyright file="CreatureTurnedNotification.cs" company="2Dudes">
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

    internal class CreatureTurnedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedNotification"/> class.
        /// </summary>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureTurnedNotification(Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureTurnedNotificationArguments arguments)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureTurnedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
        {
            if (this.Arguments.TurnedEffect != AnimatedEffect.None)
            {
                this.Packets.Add(new MagicEffectPacket(this.Arguments.Creature.Location, this.Arguments.TurnedEffect));
            }

            this.Packets.Add(new CreatureTurnedPacket(this.Arguments.Creature));
        }
    }
}