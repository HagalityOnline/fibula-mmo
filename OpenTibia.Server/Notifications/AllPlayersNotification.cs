// <copyright file="AllPlayersNotification.cs" company="2Dudes">
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

    /// <summary>
    /// Abstract class that represents a notification to a player's connection.
    /// Notifications are basically any message that the server sends to the client of a specific player.
    /// </summary>
    internal abstract class AllPlayersNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllPlayersNotification"/> class.
        /// </summary>
        /// <param name="targetConnectionsFunc">An expression to determine the target connections of this notification.</param>
        protected AllPlayersNotification(Expression<Func<IEnumerable<IConnection>>> targetConnectionsFunc)
            : base(targetConnectionsFunc)
        {
        }

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

                connections = this.TargetConnectionsFunction();

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
