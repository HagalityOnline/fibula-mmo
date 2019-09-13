// <copyright file="CreatureSpokeNotification.cs" company="2Dudes">
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

    internal class CreatureSpokeNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpokeNotification"/> class.
        /// </summary>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureSpokeNotification(Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureSpokeNotificationArguments arguments)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureSpokeNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
        {
            this.Packets.Add(new CreatureSpeechPacket(this.Arguments.Creature.Name, this.Arguments.SpeechType, this.Arguments.Message, this.Arguments.Creature.Location, this.Arguments.Channel, (uint)DateTimeOffset.UtcNow.Ticks));
        }
    }
}