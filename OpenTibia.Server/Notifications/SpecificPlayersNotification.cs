// <copyright file="SpecificPlayersNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Abstract class that represents a notification for a specific set of players.
    /// </summary>
    internal abstract class SpecificPlayersNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificPlayersNotification"/> class.
        /// </summary>
        /// <param name="targetConnectionsFunc">A reference to determine the target connections of this notification.</param>
        /// <param name="playerIds">The ids of the players that this notification is intended for.</param>
        protected SpecificPlayersNotification(Func<IEnumerable<IConnection>> targetConnectionsFunc, params Guid[] playerIds)
            : base(targetConnectionsFunc)
        {
            if (playerIds != null)
            {
                this.TargetPlayerIds = playerIds.ToHashSet();
            }
        }

        /// <summary>
        /// Gets the set of player ids that this notification is intended for.
        /// </summary>
        public ISet<Guid> TargetPlayerIds { get; }

        /// <summary>
        /// Sends the notification using the supplied connection.
        /// </summary>
        protected override void Send()
        {
            if (!this.Packets.Any() || !this.TargetPlayerIds.Any())
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

                foreach (var connection in connections.Where(c => this.TargetPlayerIds.Contains(c.PlayerId)))
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
