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
        /// <param name="playerId"></param>
        /// <param name="outgoingPackets"></param>
        public GenericNotificationArguments(params IOutgoingPacket[] outgoingPackets)
        {
            if (outgoingPackets == null || !outgoingPackets.Any())
            {
                throw new ArgumentNullException(nameof(outgoingPackets));
            }

            this.OutgoingPackets = outgoingPackets;
        }

        public IEnumerable<IOutgoingPacket> OutgoingPackets { get; }
    }
}