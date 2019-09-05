// <copyright file="BaseHandler.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that serves as the base implementation for all packet handlers in the service.
    /// </summary>
    public abstract class BaseHandler : IHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHandler"/> class.
        /// </summary>
        protected BaseHandler()
        {
            this.ResponsePackets = new List<IOutgoingPacket>();
        }

        /// <summary>
        /// Gets a value indicating whether this handler has prepared a response after handling the request.
        /// </summary>
        public bool IntendsToRespond => this.ResponsePackets.Any();

        /// <summary>
        /// Gets the type of packet that this handler is for.
        /// </summary>
        public abstract byte ForPacketType { get; }

        /// <summary>
        /// Gets the response packets that this hanlder will respond with.
        /// </summary>
        protected IList<IOutgoingPacket> ResponsePackets { get; }

        /// <summary>
        /// Handles the contents of a network message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="connection">A reference to the connection from where this message is comming from, for context.</param>
        public abstract void HandleRequest(INetworkMessage message, IConnection connection);

        /// <summary>
        /// Prepares a <see cref="INetworkMessage"/> as a response if this handler <see cref="IntendsToRespond"/>.
        /// </summary>
        /// <returns>The response as a <see cref="INetworkMessage"/>.</returns>
        public virtual INetworkMessage PrepareResponse()
        {
            if (!this.IntendsToRespond)
            {
                return null;
            }

            INetworkMessage message = new NetworkMessage();

            foreach (var outPacket in this.ResponsePackets)
            {
                outPacket.WriteToMessage(message);
            }

            return message;
        }
    }
}