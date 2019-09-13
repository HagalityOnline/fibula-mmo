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

        DateTimeOffset CurrentTime { get; }

        byte LightColor { get; }

        byte LightLevel { get; }

        WorldState Status { get; }

        /// <summary>
        /// Runs the main game processing thread which begins advancing time on the game engine.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);

        bool ScheduleEvent(IEvent newEvent, TimeSpan delay = default);



        byte[] GetMapTileDescription(uint requestingPlayerId, Location location);

        IEnumerable<uint> GetSpectatingCreatureIds(Location location);

        void NotifyAllPlayers(Func<IConnection, INotification> notificationFunc);

        void NotifySinglePlayer(IPlayer player, Func<IConnection, INotification> notificationFunc);

        void NotifySpectatingPlayers(Func<IConnection, INotification> notificationFunc, params Location[] locations);

        void OnContainerContentAdded(IContainer container, IItem item);

        void OnContainerContentRemoved(IContainer container, byte index);

        void OnContainerContentUpdated(IContainer container, byte index, IItem item);

        void RequestCombatOp(ICombatOperation newOp);

        void SignalAttackReady();

        void SignalWalkAvailable();
    }
}