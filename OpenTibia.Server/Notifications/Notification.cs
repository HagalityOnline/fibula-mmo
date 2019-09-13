// <copyright file="Notification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications;
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
        protected Notification()
            : base(EvaluationTime.OnSchedule)
        {
            this.Packets = new List<IOutgoingPacket>();
            this.ActionsOnPass.Add(new GenericEventAction(this.Send));
        }

        /// <summary>
        /// Gets or sets the packets that must be send as part of this notification.
        /// </summary>
        public IList<IOutgoingPacket> Packets { get; protected set; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected abstract Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected abstract void Prepare();

        /// <summary>
        /// Sends the notification using the supplied connection.
        /// </summary>
        protected virtual void Send()
        {
            this.Prepare();

            if (!this.Packets.Any())
            {
                // TODO: log this?
                return;
            }

            IEnumerable<IConnection> connections = null;

            try
            {
                INetworkMessage outboundMessage = new NetworkMessage();

                foreach (var packet in this.Packets)
                {
                    packet.WriteToMessage(outboundMessage);
                }

                connections = this.TargetConnectionsFunction?.Invoke();

                if (connections == null)
                {
                    // TODO: log this?
                    return;
                }

                foreach (var connection in connections)
                {
                    connection.Send(outboundMessage.Copy());
                }
            }
            catch (Exception)
            {
                // TODO: log this.
            }
        }
    }
}
