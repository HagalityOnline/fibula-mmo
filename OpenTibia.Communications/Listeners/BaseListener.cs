// <copyright file="BaseListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Security;

    /// <summary>
    /// Class that is the base implementation for all listeners.
    /// </summary>
    public abstract class BaseListener : TcpListener, IOpenTibiaListener
    {
        /// <summary>
        /// The protocol in use by this listener.
        /// </summary>
        private readonly IProtocol protocol;

        /// <summary>
        /// The DoS defender to use.
        /// </summary>
        private readonly IDoSDefender defender;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseListener"/> class.
        /// </summary>
        /// <param name="port">The port to use on this listener.</param>
        /// <param name="protocol">The protocol to use in this listener.</param>
        /// <param name="dosDefender">A reference to the DoS defender service implementation.</param>
        protected BaseListener(int port, IProtocol protocol, IDoSDefender dosDefender)
            : base(IPAddress.Any, port)
        {
            protocol.ThrowIfNull(nameof(protocol));

            this.protocol = protocol;
            this.defender = dosDefender;
        }

        /// <summary>
        /// Begins listening for requests.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous listening operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.Start();

                // Stop() makes AcceptSocketAsync() throw an ObjectDisposedException.
                cancellationToken.Register(() => this.Stop());

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        Connection connection = new Connection(await this.AcceptSocketAsync());

                        connection.ConnectionClosed += this.OnConnectionClose;
                        connection.MessageReadyToProccess += this.protocol.ProcessMessage;
                        connection.AfterMessageProcessed += this.protocol.PostProcessMessage;

                        if (this.defender.IsBlocked(connection.SocketIp))
                        {
                            // TODO: evaluate if it is worth just leaving the connection open but ignore it, so that they think they are successfully DoSing...
                            // But we would need to think if it is a connection drain attack then...
                            connection.Close();

                            continue;
                        }

                        this.defender.LogConnectionAttempt(connection.SocketIp);

                        connection.BeginStreamRead();
                    }
                    catch (ObjectDisposedException)
                    {
                        // This is normal when the listerner is stopped because of token cancellation.
                        break;
                    }
                }
            }
            catch (SocketException socEx)
            {
                // TODO: proper logging.
                Console.WriteLine(socEx.ToString());
            }
        }

        /// <summary>
        /// Handles a connection's close event.
        /// </summary>
        /// <param name="connection">The connection that was closed.</param>
        private void OnConnectionClose(IConnection connection)
        {
            if (connection == null)
            {
                // TODO: log a warning here.
                return;
            }

            // De-subscribe to this event first.
            connection.ConnectionClosed -= this.OnConnectionClose;
            connection.MessageReadyToProccess -= this.protocol.ProcessMessage;
            connection.AfterMessageProcessed -= this.protocol.PostProcessMessage;
        }
    }
}
