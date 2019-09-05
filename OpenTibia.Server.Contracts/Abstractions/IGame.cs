// -----------------------------------------------------------------
// <copyright file="IGame.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    public interface IGame
    {
        DateTimeOffset CombatSynchronizationTime { get; }

        ConcurrentDictionary<Guid, ICreature> Creatures { get; }

        DateTimeOffset CurrentTime { get; }

        byte LightColor { get; }

        byte LightLevel { get; }

        WorldState Status { get; }

        bool CanThrowBetween(Location fromLocation, Location toLocation, bool checkLineOfSight = true);

        byte[] GetMapTileDescription(uint requestingPlayerId, Location location);

        IEnumerable<uint> GetSpectatingCreatureIds(Location location);

        ITile GetTileAt(Location location);

        bool InLineOfSight(Location fromLocation, Location toLocation);

        void NotifyAllPlayers(Func<IConnection, INotification> notificationFunc);

        void NotifySinglePlayer(IPlayer player, Func<IConnection, INotification> notificationFunc);

        void NotifySpectatingPlayers(Func<IConnection, INotification> notificationFunc, params Location[] locations);

        void OnContainerContentAdded(IContainer container, IItem item);

        void OnContainerContentRemoved(IContainer container, byte index);

        void OnContainerContentUpdated(IContainer container, byte index, IItem item);

        IEnumerable<Direction> Pathfind(Location startLocation, Location targetLocation, out Location endLocation, int maxStepsCount = 100);

        void RequestCombatOp(ICombatOperation newOp);

        bool ScheduleEvent(IEvent newEvent, TimeSpan delay = default);

        void SignalAttackReady();

        void SignalWalkAvailable();

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}