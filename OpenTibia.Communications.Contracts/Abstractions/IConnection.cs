﻿// -----------------------------------------------------------------
// <copyright file="IConnection.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using System;
    using OpenTibia.Communications.Contracts.Delegates;

    /// <summary>
    /// Interface for connections.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Event fired when this connection has been closed.
        /// </summary>
        event OnConnectionClosed ConnectionClosed;

        /// <summary>
        /// Event fired right after this connection has had it's <see cref="IConnection.InboundMessage"/> proccessed by any subscriber of the <see cref="MessageReadyToProccess"/> event.
        /// </summary>
        event OnMessageProccessed AfterMessageProcessed;

        /// <summary>
        /// Event fired when this connection has it's <see cref="IConnection.InboundMessage"/> ready to be proccessed.
        /// </summary>
        event OnMessageReadyToProccess MessageReadyToProccess;

        /// <summary>
        /// Gets the inbound message in this connection.
        /// </summary>
        INetworkMessage InboundMessage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this connection is authenticated.
        /// </summary>
        bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the Id of the player that this connection is associated to.
        /// </summary>
        Guid PlayerId { get; set; }

        /// <summary>
        /// Gets the Socket IP address of this connection, if it is open.
        /// </summary>
        string SocketIp { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is orphan.
        /// </summary>
        bool IsOrphaned { get; }

        /// <summary>
        /// Gets or sets this connection's XTea key.
        /// </summary>
        uint[] XTeaKey { get; set; }

        void BeginStreamRead();

        void Close();

        /// <summary>
        /// Sends a notification via this connection.
        /// </summary>
        /// <param name="notification">The notification to send.</param>
        void Send(INotification notification);

        void Send(INetworkMessage message);

        void Send(INetworkMessage message, bool useEncryption, bool managementProtocol = false);
    }
}