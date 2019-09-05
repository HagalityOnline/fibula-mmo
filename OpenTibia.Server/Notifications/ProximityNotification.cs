// <copyright file="ProximityNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Abstract class that represents a notification to a player's connection.
    /// Notifications are basically any message that the server sends to the client of a specific player.
    /// </summary>
    internal abstract class ProximityNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProximityNotification"/> class.
        /// </summary>
        /// <param name="location">The location which is being spectated.</param>
        protected ProximityNotification(Func<IEnumerable<IConnection>> targetConnectionsFunc, Location location)
            : base(targetConnectionsFunc)
        {
            this.AtLocation = location;
        }

        /// <summary>
        /// Gets the location which is being spectated.
        /// </summary>
        public Location AtLocation { get; }

        /// <summary>
        /// Sends the notification using the supplied connection.
        /// </summary>
        protected override void Send()
        {
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

                connections = this.TargetConnectionsFunction.Invoke();

                foreach (var connection in connections)
                {
                    if (this.Game.)

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
