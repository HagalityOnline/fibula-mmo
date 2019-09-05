﻿// -----------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a standard manager for connections which is thread safe.
    /// </summary>
    public class ConnectionManager : IConnectionManager
    {
        /// <summary>
        /// Gets the <see cref="IDictionary{TKey,TValue}"/> of all <see cref="IConnection"/>s in the game, in which the Key is the <see cref="IConnection.PlayerId"/>.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, IConnection> connectionsMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionManager"/> class.
        /// </summary>
        public ConnectionManager()
        {
            this.connectionsMap = new ConcurrentDictionary<Guid, IConnection>();
        }

        /// <summary>
        /// Registers a new connection to the manager.
        /// </summary>
        /// <param name="connection">The connection to register.</param>
        public void Register(IConnection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            this.connectionsMap.TryAdd(connection.PlayerId, connection);
        }

        /// <summary>
        /// Unregisters a connection from the manager.
        /// </summary>
        /// <param name="connection">The connection to unregister.</param>
        public void Unregister(IConnection connection)
        {
            connection.ThrowIfNull(nameof(connection));

            this.connectionsMap.TryRemove(connection.PlayerId, out _);
        }

        /// <summary>
        /// Looks for a single connection with the associated player id.
        /// </summary>
        /// <param name="playerId">The player id for which to look a connection for.</param>
        /// <returns>The connection instance, if found, and null otherwise.</returns>
        public IConnection FindByPlayerId(Guid playerId)
        {
            if (this.connectionsMap.TryGetValue(playerId, out IConnection connection))
            {
                return connection;
            }

            return null;
        }

        /// <summary>
        /// Gets all active connections known to this manager.
        /// </summary>
        /// <returns>A collection of connection instances.</returns>
        public IEnumerable<IConnection> GetAllActive()
        {
            // ConcurrentDictionary.Values does produce a moment-in-time snapshot, so no need to do ToList().
            return this.connectionsMap.Values.Where(c => !c.IsOrphaned);
        }

        /// <summary>
        /// Gets all orphaned connections known to this manager.
        /// </summary>
        /// <returns>A collection of connection instances.</returns>
        public IEnumerable<IConnection> GetAllOrphaned()
        {
            // ConcurrentDictionary.Values does produce a moment-in-time snapshot, so no need to do ToList().
            return this.connectionsMap.Values.Where(c => c.IsOrphaned);
        }
    }
}