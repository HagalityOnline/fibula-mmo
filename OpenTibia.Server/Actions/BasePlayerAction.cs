// <copyright file="BasePlayerAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Actions
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Notifications;

    internal abstract class BasePlayerAction : IAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePlayerAction"/> class.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="packet"></param>
        /// <param name="retryLocation"></param>
        protected BasePlayerAction(IPlayer player, IIncomingPacket packet, Location retryLocation)
        {
            player.ThrowIfNull(nameof(player));
            packet.ThrowIfNull(nameof(packet));

            this.Player = player;
            this.Packet = packet;
            this.RetryLocation = retryLocation;
            this.ResponsePackets = new List<IOutgoingPacket>();
        }

        public IPlayer Player { get; }

        public IIncomingPacket Packet { get; }

        public Location RetryLocation { get; }

        public IList<IOutgoingPacket> ResponsePackets { get; }

        public void Perform()
        {
            this.InternalPerform();

            if (this.ResponsePackets.Any())
            {
                Game.Instance.NotifySinglePlayer(this.Player, conn => new GenericNotification(conn, this.ResponsePackets.Cast<PacketOutgoing>().ToArray()));
            }
        }

        protected abstract void InternalPerform();
    }
}