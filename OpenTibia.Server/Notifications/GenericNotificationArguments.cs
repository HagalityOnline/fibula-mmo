// <copyright file="GenericNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class GenericNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNotificationArguments"/> class.
        /// </summary>
        /// <param name="outgoingPackets">The packets to send as part of this notification.</param>
        /// <param name="playerIds">The ids of the players that this notification is intended for.</param>
        public GenericNotificationArguments(IEnumerable<IOutgoingPacket> outgoingPackets, params Guid[] playerIds)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            if (playerIds == null || !playerIds.Any())
            {
                throw new ArgumentNullException(nameof(playerIds));
            }

            this.OutgoingPackets = outgoingPackets;
            this.PlayerIds = playerIds;
        }

        /// <summary>
        /// Gets the packets to be included in this notification.
        /// </summary>
        public IEnumerable<IOutgoingPacket> OutgoingPackets { get; }

        /// <summary>
        /// Gets the ids of the players for which the notification is intended for.
        /// </summary>
        public IEnumerable<Guid> PlayerIds { get; }
    }
}