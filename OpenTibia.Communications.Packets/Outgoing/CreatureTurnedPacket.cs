﻿// -----------------------------------------------------------------
// <copyright file="CreatureTurnedPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Outgoing
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    public class CreatureTurnedPacket : IOutgoingPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedPacket"/> class.
        /// </summary>
        /// <param name="creature">The creature that turned.</param>
        /// <param name="creatureStackPosition">The position in the stack of the creature that turned.</param>
        public CreatureTurnedPacket(ICreature creature, byte creatureStackPosition)
        {
            this.Creature = creature;
            this.StackPosition = creatureStackPosition;
        }

        /// <summary>
        /// Gets the type of this packet.
        /// </summary>
        public byte PacketType => (byte)OutgoingGamePacketType.TransformThing;

        /// <summary>
        /// Gets the creature that turned.
        /// </summary>
        public ICreature Creature { get; }

        /// <summary>
        /// Gets the position in the stack of the creatue.
        /// </summary>
        public byte StackPosition { get; }

        /// <summary>
        /// Writes the packet to the message provided.
        /// </summary>
        /// <param name="message">The message to write this packet to.</param>
        public void WriteToMessage(INetworkMessage message)
        {
            message.WriteCreatureTurnedPacket(this);
        }
    }
}
