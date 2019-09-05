// <copyright file="Notification.cs" company="2Dudes">
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
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts.Enumerations;

    /// <summary>
    /// Abstract class that represents a notification to a player's connection.
    /// Notifications are basically any message that the server sends to the client of a specific player.
    /// </summary>
    internal abstract class Notification : BaseEvent, INotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Notification"/> class.
        /// </summary>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        protected Notification(Func<IEnumerable<IConnection>> determineTargetConnectionsFunction)
            : base(EvaluationTime.OnSchedule)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));

            this.Packets = new List<IOutgoingPacket>();
            this.ActionsOnPass.Add(new GenericEventAction(this.Send));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
        }

        /// <summary>
        /// Gets or sets the packets that must be send as part of this notification.
        /// </summary>
        public IList<IOutgoingPacket> Packets { get; protected set; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        /// Sends the notification using the supplied connection.
        /// </summary>
        protected abstract void Send();
    }
}
