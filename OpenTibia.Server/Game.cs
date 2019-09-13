// <copyright file="Game.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Common.Utilities;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Data.Models;
    using OpenTibia.Scheduling.Contracts;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Algorithms;
    using OpenTibia.Server.Combat;
    using OpenTibia.Server.Contracts;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Models;
    using OpenTibia.Server.Movement;
    using OpenTibia.Server.Notifications;

    /// <summary>
    /// Main class.
    /// </summary>
    public class Game : IGame, ICreatureManager
    {
        /// <summary>
        /// The default adavance time step span.
        /// </summary>
        private const int GameStepSizeInMilliseconds = 500;

        /// <summary>
        /// Defines the <see cref="TimeSpan"/> to wait between checks for orphaned conections.
        /// </summary>
        private static readonly TimeSpan CheckOrphanConnectionsDelay = TimeSpan.FromSeconds(1);

        private readonly object attackLock;
        private readonly object walkLock;
        private readonly object combatQueueLock;

        /// <summary>
        /// Gets the current <see cref="map"/> instance.
        /// </summary>
        private readonly IMap map;

        /// <summary>
        /// Stores a reference to the global scheduler.
        /// </summary>
        private readonly IScheduler scheduler;

        /// <summary>
        /// Stores a reference to the pathfinder helper algorithm.
        /// </summary>
        private readonly IPathFinder pathFinder;

        /// <summary>
        /// Gets the <see cref="IDictionary{TKey,TValue}"/> containing the <see cref="IItemEvent"/>s of the game.
        /// </summary>
        private readonly IDictionary<ItemEventType, HashSet<IItemEvent>> eventsCatalog;

        private readonly IItemEventLoader eventLoader;

        private readonly IItemLoader itemLoader;

        private readonly IMonsterLoader monsterLoader;

        /// <summary>
        /// Holds the <see cref="ConcurrentDictionary{TKey,TValue}"/> of all <see cref="Creature"/>s in the game, in which the Key is the <see cref="Creature.Id"/>.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, ICreature> creatureMap;

        /// <summary>
        /// Stores the current world status.
        /// </summary>
        private WorldState status;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game(
            IMap map,
            IItemEventLoader itemEventLoader,
            IItemLoader itemLoader,
            IMonsterLoader monsterLoader,
            IScheduler scheduler,
            IConnectionManager connectionManager,
            INotificationFactory notificationFactory,
            ICreatureFactory creatureFactory)
        {
            this.attackLock = new object();
            this.walkLock = new object();
            this.combatQueueLock = new object();

            this.ConnectionManager = connectionManager;
            this.CreatureFactory = creatureFactory;
            this.NotificationFactory = notificationFactory;

            this.creatureMap = new ConcurrentDictionary<Guid, ICreature>();
            this.CombatQueue = new ConcurrentQueue<ICombatOperation>();

            // Initialize the map
            this.map = map;
            this.scheduler = scheduler;
            this.eventLoader = itemEventLoader;
            this.itemLoader = itemLoader;
            this.monsterLoader = monsterLoader;

            // Initialize game vars.
            this.Status = WorldState.Creating;
            this.LightColor = (byte)LightColors.White;
            this.LightLevel = (byte)LightLevels.World;

            this.eventsCatalog = this.eventLoader.Load(ServerConfiguration.MoveUseFileName);

            this.scheduler.OnEventFired += this.ProcessFiredEvent;
        }

        /// <summary>
        /// Gets the current time in the game.
        /// </summary>
        public DateTimeOffset CurrentTime { get; private set; }

        /// <summary>
        /// Gets the current world's light level <see cref="byte"/> value.
        /// </summary>
        public byte LightLevel { get; private set; }

        /// <summary>
        /// Gets the current world's light color <see cref="byte"/> value.
        /// </summary>
        public byte LightColor { get; private set; }

        /// <summary>
        /// Gets the current world's <see cref="WorldState"/>.
        /// </summary>
        public WorldState Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                this.status = value;
                Console.WriteLine($"Game world is now {this.status}.");
            }
        }

        /// <summary>
        /// Gets the reference to the connection manager instance.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        public IAssetManagementContext AssetManagement { get; }

        /// <summary>
        /// Gets the reference to the notification factory instance.
        /// </summary>
        public INotificationFactory NotificationFactory { get; }



        public DateTimeOffset CombatSynchronizationTime { get; private set; }

        public ICreatureFactory CreatureFactory { get; }

        public IScriptFactory ScriptFactory { get; private set; }

        /// <summary>
        /// Gets the current <see cref="ConcurrentQueue{T}"/> of <see cref="ICombatOperation"/>s, which the game processes on the <see cref="CombatProcessor"/> method.
        /// </summary>
        private ConcurrentQueue<ICombatOperation> CombatQueue { get; }

        /// <summary>
        /// Runs the main game processing thread which begins advancing time on the game engine.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var stepSize = TimeSpan.FromMilliseconds(GameStepSizeInMilliseconds);

            var connectionSweepperTask = Task.Factory.StartNew(this.ConnectionSweeper, TaskCreationOptions.LongRunning);

            // Leave this at the very end, when everything is ready...
            this.Status = WorldState.Open;

            while (!cancellationToken.IsCancellationRequested)
            {
                var timeAtStart = DateTimeOffset.UtcNow;

                this.AdvanceTime(stepSize);

                var timeThatCheckTook = DateTimeOffset.UtcNow - timeAtStart;
                var actualDelay = stepSize - timeThatCheckTook;

                if (actualDelay > TimeSpan.Zero)
                {
                    await Task.Delay(actualDelay, cancellationToken);
                }
                else
                {
                    // TODO: proper logging.
                    Console.WriteLine($"WARN: Time is slipping. It took {timeThatCheckTook} to advance time with a step size of {stepSize}.");
                }
            }

            await Task.WhenAll(connectionSweepperTask);
        }

        public bool ScheduleEvent(IEvent newEvent, TimeSpan delay = default)
        {
            newEvent.ThrowIfNull(nameof(newEvent));

            // Check if the event can be executed if explicitly set to OnShchedule or OnBoth.
            if ((newEvent.EvaluateAt == EvaluationTime.OnSchedule ||
                 newEvent.EvaluateAt == EvaluationTime.OnBoth) && !newEvent.CanBeExecuted)
            {
                return false;
            }

            var noDelay = delay == default || delay < TimeSpan.Zero;

            if (noDelay)
            {
                this.scheduler.ImmediateEvent(newEvent);
            }
            else
            {
                this.scheduler.ScheduleEvent(newEvent, DateTimeOffset.UtcNow + delay);
            }

            return true;
        }

        /// <summary>
        /// Advances Time in the game by the supplied span.
        /// </summary>
        /// <param name="timeStep">The span of time to advance the game for.</param>
        private void AdvanceTime(TimeSpan timeStep)
        {
            // store the current time for global reference and actual calculations.
            this.CurrentTime = DateTimeOffset.UtcNow;

            var eventsToSchedule =

                // handle all creature thinking and decision making.
                this.AdvanceTimeForThinking(timeStep)

                // handle speech for everything.
                .Union(this.AdvanceTimeForSpeech(timeStep))

                // handle all creature moving and actions.
                .Union(this.AdvanceTimeForMoving(timeStep))

                // handle combat.
                .Union(this.AdvanceTimeForCombat(timeStep))

                // handle miscellaneous things like day cycle.
                .Union(this.AdvanceTimeForMiscellaneous(timeStep));

            foreach (var (evt, delay) in eventsToSchedule)
            {
                this.ScheduleEvent(evt, delay - (DateTimeOffset.UtcNow - this.CurrentTime));
            }
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForThinking(TimeSpan stepSize)
        {
            var eventsToSchedule = new List<(IEvent Event, TimeSpan Delay)>();

            foreach (var creature in this.creatureMap.Values)
            {
                // TODO: make creature think here
                // Schedule any actions that the creature would take here.
                var decisionsAndActions = creature.Think();

                if (decisionsAndActions != null && decisionsAndActions.Any())
                {
                    eventsToSchedule.AddRange(decisionsAndActions);
                }
            }

            return eventsToSchedule;
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForSpeech(TimeSpan timeStep)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForMoving(TimeSpan timeStep)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForCombat(TimeSpan timeStep)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<(IEvent Event, TimeSpan Delay)> AdvanceTimeForMiscellaneous(TimeSpan timeStep)
        {
            var events = new List<(IEvent, TimeSpan)>();

            const int NightLightLevel = 30;
            const int DuskDawnLightLevel = 130;
            const int DayLightLevel = 255;

            // A day is roughly an hour in real time, and night lasts roughly 1/3 of the day in real time
            // Dusk and Dawns last for 30 minutes roughly, so les aproximate that to 2 minutes.
            var currentMinute = this.CurrentTime.Minute;

            if (currentMinute >= 0 && currentMinute <= 37)
            {
                // Day time: [0, 37] minutes on the hour.
                if (this.LightLevel != DayLightLevel)
                {
                    this.LightLevel = DayLightLevel;
                    this.LightColor = (byte)LightColors.White;

                    events.Add((this.NotificationFactory.Create(NotificationType.WorldLightChanged, new WorldLightChangedNotificationArguments(this.LightLevel, this.LightColor)) as IEvent, TimeSpan.Zero));
                }
            }
            else if (currentMinute == 38 || currentMinute == 39 || currentMinute == 58 || currentMinute == 59)
            {
                // Dusk: [38, 39] minutes on the hour.
                // Dawn: [58, 59] minutes on the hour.
                if (this.LightLevel != DuskDawnLightLevel)
                {
                    this.LightLevel = DuskDawnLightLevel;
                    this.LightColor = (byte)LightColors.Orange;

                    events.Add((this.NotificationFactory.Create(NotificationType.WorldLightChanged, new WorldLightChangedNotificationArguments(this.LightLevel, this.LightColor)) as IEvent, TimeSpan.Zero));
                }
            }
            else if (currentMinute >= 40 && currentMinute <= 57)
            {
                // Night time: [40, 57] minutes on the hour.
                if (this.LightLevel != NightLightLevel)
                {
                    this.LightLevel = NightLightLevel;
                    this.LightColor = (byte)LightColors.White;

                    events.Add((this.NotificationFactory.Create(NotificationType.WorldLightChanged, new WorldLightChangedNotificationArguments(this.LightLevel, this.LightColor)) as IEvent, TimeSpan.Zero));
                }
            }

            return events;
        }

        /// <summary>
        /// Cleans up stale connections.
        /// </summary>
        /// <param name="tokenState"></param>
        private void ConnectionSweeper(object tokenState)
        {
            var cancellationToken = (tokenState as CancellationToken?).Value;

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(CheckOrphanConnectionsDelay);

                foreach (var orphanedConnection in this.ConnectionManager.GetAllOrphaned())
                {
                    if (!(this.FindCreatureById(orphanedConnection.PlayerId) is IPlayer player))
                    {
                        continue;
                    }

                    player.SetAttackTarget(0);

                    if (player.IsLogoutAllowed)
                    {
                        player.AttemptLogout();
                    }
                }
            }
        }

        // Movement thread
        private void ProcessFiredEvent(object sender, EventFiredEventArgs eventArgs)
        {
            if (sender != this.scheduler || eventArgs?.Event == null)
            {
                return;
            }

            IEvent evt = eventArgs.Event;

            Console.WriteLine($"Processing event {evt.EventId}.");

            try
            {
                evt.Process();
                Console.WriteLine($"Processed event {evt.EventId}.");
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        #region Asset Management

        public void AddThingToTile(ref IThing thingToAdd, ITile toTile)
        {
            thingToAdd.ThrowIfNull(nameof(thingToAdd));
            toTile.ThrowIfNull(nameof(toTile));

            toTile.AddThing(ref thingToAdd);

            this.NotificationFactory.Create(
                NotificationType.TileUpdated,
                new TileUpdatedNotificationArguments(toTile.Location, this.GetMapTileDescription()));

            this.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), toTile.Location);
        }
        #endregion
















        public byte[] GetMapTileDescription(Guid requestingPlayerId, Location location)
        {
            var tile = this.map[location];

            if (!(this.FindCreatureById(requestingPlayerId) is IPlayer requestingPlayer))
            {
                return new byte[0];
            }

            return tile == null ? new byte[0] : this.map.GetTileDescription(requestingPlayer, tile).ToArray();
        }

        public void SignalWalkAvailable()
        {
            lock (this.walkLock)
            {
                Monitor.Pulse(this.walkLock);
            }
        }

        public void SignalAttackReady()
        {
            lock (this.attackLock)
            {
                Monitor.Pulse(this.attackLock);
            }
        }

        public void RequestCombatOp(ICombatOperation newOp)
        {
            lock (this.combatQueueLock)
            {
                this.CombatQueue.Enqueue(newOp);

                Monitor.Pulse(this.combatQueueLock);
            }
        }

        public void NotifySinglePlayer(IPlayer player, Func<IConnection, INotification> notificationFunc)
        {
            if (player == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(player)} on {nameof(this.NotifySinglePlayer)}.");
                return;
            }

            if (notificationFunc == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(notificationFunc)} on {nameof(this.NotifySinglePlayer)}.");
                return;
            }

            try
            {
                var conn = this.ConnectionManager.FindByPlayerId(player.Id);

                this.InternalRequestNofitication(notificationFunc(conn));
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void NotifyAllPlayers(Func<IConnection, INotification> notificationFunc)
        {
            if (notificationFunc == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(notificationFunc)} on NotifyAllPlayers.");
                return;
            }

            foreach (var conn in this.ConnectionManager.GetAllActive())
            {
                this.InternalRequestNofitication(notificationFunc(conn));
            }
        }

        public void NotifySpectatingPlayers(Func<IConnection, INotification> notificationFunc, params Location[] locations)
        {
            if (notificationFunc == null)
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: null {nameof(notificationFunc)} on {nameof(this.NotifySpectatingPlayers)}.");
                return;
            }

            if (!locations.Any())
            {
                // TODO: proper logging
                Console.WriteLine($"WARN: no locations provided for notification.");
                return;
            }

            var allSpectating = this.GetSpectatingPlayers(locations.First());

            allSpectating = locations.Skip(1).Aggregate(allSpectating, (current, otherLocation) => current.Union(this.GetSpectatingPlayers(otherLocation), new CreatureEqualityComparer()).Select(c => c as Player));

            foreach (var spectator in allSpectating)
            {
                try
                {
                    var conn = this.ConnectionManager.FindByPlayerId(spectator.Id);

                    this.InternalRequestNofitication(notificationFunc(conn));
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void CheckCreatureWalk()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                try
                {
                    var minCoolDown = TimeSpan.MaxValue;

                    foreach (var creature in this.creatureMap.Values.Where(c => c.WalkingQueue.Count > 0).ToList())
                    {
                        var cooldownTime = creature.CalculateRemainingCooldownTime(ExhaustionType.Move, this.CurrentTime);

                        if (cooldownTime <= TimeSpan.Zero && creature.WalkingQueue.TryPeek(out Tuple<byte, Direction> nextTuple) && creature.NextStepId == nextTuple.Item1)
                        {
                            // Time to walk, let's process it.
                            if (!creature.WalkingQueue.TryDequeue(out nextTuple))
                            {
                                continue;
                            }

                            if (!this.RequestCreatureWalkToDirection(creature, nextTuple.Item2) && creature is IPlayer)
                            {
                                creature.StopWalking();

                                if (creature is IPlayer player)
                                {
                                    this.InternalRequestNofitication(
                                        this.NotificationFactory.Create(
                                            NotificationType.Generic,
                                            new GenericNotificationArguments(
                                                new PlayerWalkCancelPacket(player.Direction),
                                                new TextMessagePacket(MessageType.StatusSmall, "Sorry, not possible."))));
                                }
                            }

                            // recalc cooldown for this creature
                            cooldownTime = creature.CalculateRemainingCooldownTime(ExhaustionType.Move, this.CurrentTime);

                            if (creature.WalkingQueue.Count > 0 && cooldownTime < minCoolDown)
                            {
                                minCoolDown = cooldownTime;
                            }
                        }
                        else if (cooldownTime < minCoolDown)
                        {
                            minCoolDown = cooldownTime;
                        }
                    }

                    lock (this.walkLock)
                    {
                        if (minCoolDown != TimeSpan.MaxValue)
                        {
                            var timeThatCheckTook = DateTimeOffset.UtcNow - this.CurrentTime;
                            var timeDiff = minCoolDown - timeThatCheckTook; // factor in the time we took to check all queues.
                            var actualCooldown = timeDiff > TimeSpan.Zero ? timeDiff : TimeSpan.Zero; // and if that is positive.

                            Monitor.Wait(this.walkLock, actualCooldown); // there was work, but it's not time yet.
                        }
                        else
                        {
                            Monitor.Wait(this.walkLock); // there was no work, sleep until woken up.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void CheckCreatureAutoAttack()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                try
                {
                    this.CombatSynchronizationTime = DateTimeOffset.UtcNow;
                    var minCoolDown = TimeSpan.MaxValue;

                    foreach (var creature in this.creatureMap.Values.Where(c => c.AutoAttackTargetId > 0).ToList())
                    {
                        var cooldownTime = creature.CalculateRemainingCooldownTime(ExhaustionType.Combat, this.CombatSynchronizationTime);

                        if (cooldownTime <= TimeSpan.Zero)
                        {
                            // reset the cooldown time to max so that we don't count this
                            cooldownTime = TimeSpan.MaxValue;

                            // Time to attack, let's process it.
                            if (this.FindCreatureById(creature.AutoAttackTargetId) is Creature target)
                            {
                                var standardAttackOp = new StandardAttackOperation(creature, target);

                                if (standardAttackOp.CanBeExecuted) // pre check
                                {
                                    this.RequestCombatOp(standardAttackOp);
                                    cooldownTime = standardAttackOp.ExhaustionCost;
                                }
                            }
                            else
                            {
                                // creature is not on the global creature's list, hence it's invalid now.
                                creature.SetAttackTarget(0);
                            }
                        }

                        if (cooldownTime < minCoolDown)
                        {
                            minCoolDown = cooldownTime;
                        }
                    }

                    lock (this.attackLock)
                    {
                        if (minCoolDown != TimeSpan.MaxValue)
                        {
                            var timeThatCheckTook = DateTimeOffset.UtcNow - this.CombatSynchronizationTime;
                            var timeDiff = minCoolDown - timeThatCheckTook; // factor in the time we took to check all queues.
                            var actualCooldown = timeDiff > TimeSpan.Zero ? timeDiff : TimeSpan.Zero; // and if that is positive.

                            Monitor.Wait(this.attackLock, actualCooldown); // there was work, but it's not time yet.
                        }
                        else
                        {
                            Monitor.Wait(this.attackLock); // there was no work, sleep until woken up.
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void PlaceMonsters()
        {
            var monsterSpawns = this.monsterLoader.LoadSpawns(ServerConfiguration.SpawnsFileName);
            var loadedCount = 0;
            var spawnsList = monsterSpawns as IList<Spawn> ?? monsterSpawns.ToList();
            var totalCount = spawnsList.Sum(s => s.Count);

            var percentageCompleteFunc = new Func<byte>(() => (byte)Math.Floor(Math.Min(100, Math.Max(0M, loadedCount * 100 / (totalCount + 1)))));
            var cts = new CancellationTokenSource();

            Task.Factory.StartNew(
            () =>
            {
                while (!cts.IsCancellationRequested)
                {
                    // TODO: proper logging
                    Console.WriteLine($"Monster placed percentage: {percentageCompleteFunc()} %");
                    Thread.Sleep(TimeSpan.FromSeconds(7)); // TODO: this is arbitrary...
                }
            }, cts.Token);

            var rng = new Random();
            Parallel.ForEach(spawnsList, spawn =>
            {
                Parallel.For(0, spawn.Count, i =>
                {
                    // var halfRadius = spawn.Radius / 2;
                    var placed = false;

                    byte tries = 1;
                    do
                    {
                        // TODO: revisit this logic
                        var randomLoc = spawn.Location + new Location { X = (int)Math.Round((i * Math.Cos(rng.Next(360))) - i), Y = (int)Math.Round((i * Math.Cos(rng.Next(360))) - i), Z = 0 };
                        var randomTile = this.GetTileAt(randomLoc);

                        if (randomTile == null)
                        {
                            continue;
                        }

                        // Need to actually pathfind to avoid placing a monster in unreachable places.
                        this.pathFinder.FindBetween(spawn.Location, randomTile.Location, out Location foundLocation, (i + 1) * 100);

                        var foundTile = this.GetTileAt(foundLocation);

                        if (foundTile != null && !foundTile.BlocksPass)
                        {
                            this.ScriptFactory.MonsterOnMap(this, foundLocation, spawn.Id);
                            placed = true;
                        }
                    }
                    while (++tries != 0 && !placed);

                    if (!placed)
                    {
                        // TODO: proper logging
                        Console.WriteLine($"Given up on placing monster with type {spawn.Id} around {spawn.Location}, no suitable tile found.");
                    }

                    Interlocked.Add(ref loadedCount, 1);
                });
            });

            // cancel the progress thread.
            cts.Cancel();
        }

        private void CombatProcessor()
        {
            while (!this.CancelToken.IsCancellationRequested)
            {
                ICombatOperation combatOp;

                lock (this.combatQueueLock)
                {
                    while (this.CombatQueue.Count == 0 || !this.CombatQueue.TryDequeue(out combatOp))
                    {
                        Monitor.Wait(this.combatQueueLock);
                    }
                }

                try
                {
                    if (combatOp.CanBeExecuted)
                    {
                        var canAttack = true;

                        if (combatOp.Target is ICreature targetAsCreature && combatOp.Attacker is ICreature attackerAsCreature)
                        {
                            canAttack &= attackerAsCreature.CanSee(targetAsCreature);
                            canAttack &= this.CanThrowBetween(attackerAsCreature.Location, targetAsCreature.Location);
                        }

                        if (canAttack)
                        {
                            combatOp.Execute();
                        }
                    }

                    // CombatOperations that cannot be performed are discarded nontheless.
                }
                catch (Exception ex)
                {
                    // TODO: proper logging
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public IEnumerable<uint> GetSpectatingCreatureIds(Location location)
        {
            return this.map.GetCreatureIdsAt(location);
        }

        private IEnumerable<IPlayer> GetSpectatingPlayers(Location location)
        {
            return this.ConnectionManager.GetAllAliveCreatureIds()
                .Select(creatureId => this.GetCreatureWithId(creatureId))
                .Where(c => c.CanSee(location))
                .Cast<IPlayer>()
                .ToList();
        }

        internal Player Login(PlayerModel playerRecord, Connection connection)
        {
            var player = this.CreatureFactory.Create(this, CreatureType.Player, new PlayerMetadata(playerRecord.Charname, 100, 100, 100, 100, 4240)) as Player;

            // TODO: check if map.CanAddCreature(playerRecord.location);
            // playerRecord.location
            IThing playerThing = player;
            this.map[Mapping.Map.VeteranStart].AddThing(ref playerThing);

            this.NotifySpectatingPlayers(conn => new CreatureAddedNotification(player.Id, player, AnimatedEffect.BubbleBlue), player.Location);

            this.ConnectionManager.Register(connection);

            if (!this.creatureMap.TryAdd(player.Id, player))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {player.Name} to the global dictionary.");
            }

            return player;
        }

        internal byte[] GetMapDescriptionAt(IPlayer forPlayer, Location location)
        {
            return this.map.GetDescription(forPlayer, (ushort)(location.X - 8), (ushort)(location.Y - 6), location.Z, location.IsUnderground).ToArray(); // TODO: handle near top left of map edge case.
        }

        internal byte[] GetMapDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX, byte windowSizeY)
        {
            return this.map.GetDescription(player, fromX, fromY, currentZ, isUnderground, windowSizeX, windowSizeY).ToArray();
        }

        internal byte[] GetMapFloorsDescription(IPlayer forPlayer, ushort fromX, ushort fromY, short startZ, short endZ, byte windowSizeX, byte windowSizeY, int startingOffsetZ = 0)
        {
            var totalBytes = new List<byte>();

            var skip = -1;
            var stepZ = 1; // asume going down the ground

            if (startZ > endZ)
            {
                stepZ = -1; // we're going up!
            }

            for (int currentZ = startZ; currentZ != endZ + stepZ; currentZ += stepZ)
            {
                totalBytes.AddRange(this.map.GetFloorDescription(forPlayer, fromX, fromY, (sbyte)currentZ, windowSizeX, windowSizeY, startZ - currentZ + startingOffsetZ, ref skip));
            }

            if (skip >= 0)
            {
                totalBytes.Add((byte)skip);
                totalBytes.Add(0xFF);
            }

            return totalBytes.ToArray();
        }

        internal bool RequestCreatureWalkToDirection(ICreature creature, Direction direction, TimeSpan delay = default)
        {
            var fromLoc = creature.Location;
            var toLoc = fromLoc;

            switch (direction)
            {
                case Direction.North:
                    toLoc.Y -= 1;
                    break;
                case Direction.South:
                    toLoc.Y += 1;
                    break;
                case Direction.East:
                    toLoc.X += 1;
                    break;
                case Direction.West:
                    toLoc.X -= 1;
                    break;
                case Direction.NorthEast:
                    toLoc.X += 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.NorthWest:
                    toLoc.X -= 1;
                    toLoc.Y -= 1;
                    break;
                case Direction.SouthEast:
                    toLoc.X += 1;
                    toLoc.Y += 1;
                    break;
                case Direction.SouthWest:
                    toLoc.X -= 1;
                    toLoc.Y += 1;
                    break;
            }

            var movement = new CreatureMovementOnMap(creature.Id, creature, fromLoc, toLoc);

            return this.ScheduleEvent(movement, delay);
        }

        internal void TestingViaCreatureSpeech(IPlayer player, string msgStr)
        {
            var testStrObjs = msgStr.Replace("test.", string.Empty).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (!testStrObjs.Any())
            {
                return;
            }

            switch (testStrObjs[0])
            {
                case "mon":
                    if (testStrObjs.Length > 1)
                    {
                        var monsterId = Convert.ToUInt16(testStrObjs[1]);

                        this.ScriptFactory.MonsterOnMap(this, player.LocationInFront, monsterId);
                    }

                    break;
            }
        }

        private void OnContainerContentEvent(sbyte operationType, IContainer container, byte index, IItem item)
        {
            var interestedCreatureIds = container.OpenedBy.Keys.ToList();

            foreach (var peekerId in interestedCreatureIds)
            {
                if (!(this.FindCreatureById(peekerId) is IPlayer peeker))
                {
                    continue;
                }

                switch (operationType)
                {
                    default:
                        // update
                        this.NotifySinglePlayer(peeker, conn => new GenericNotification(this.ConnectionManager.GetAllActive, new GenericNotificationArguments(new ContainerUpdateItemPacket(index, (byte)container.GetIdFor(peekerId), item), peekerId)));
                        break;
                    case -1:
                        // remove
                        this.NotifySinglePlayer(peeker, conn => new GenericNotification(this.ConnectionManager.GetAllActive, new GenericNotificationArguments(new ContainerRemoveItemPacket(index, (byte)container.GetIdFor(peekerId)), peekerId)));
                        break;
                    case 1:
                        // add
                        this.NotifySinglePlayer(peeker, conn => new GenericNotification(this.ConnectionManager.GetAllActive, new GenericNotificationArguments(new ContainerAddItemPacket((byte)container.GetIdFor(peekerId), item), peekerId)));
                        break;
                }
            }
        }

        public void OnContainerContentUpdated(IContainer container, byte index, IItem item)
        {
            container.ThrowIfNull(nameof(container));
            item.ThrowIfNull(nameof(item));

            this.OnContainerContentEvent(0, container, index, item);
        }

        public void OnContainerContentAdded(IContainer container, IItem item)
        {
            container.ThrowIfNull(nameof(container));
            item.ThrowIfNull(nameof(item));

            this.OnContainerContentEvent(1, container, 0xFF, item);
        }

        public void OnContainerContentRemoved(IContainer container, byte index)
        {
            container.ThrowIfNull(nameof(container));

            this.OnContainerContentEvent(-1, container, index, null);
        }

        /// <summary>
        /// Registers a new creature to the manager.
        /// </summary>
        /// <param name="creature">The creature to register.</param>
        public void RegisterCreature(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            if (!this.creatureMap.TryAdd(creature.Id, creature))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {creature.Name} ({creature.Id}) to the creatue manager.");
            }
        }

        /// <summary>
        /// Unregisters a creature from the manager.
        /// </summary>
        /// <param name="creature">The creature to unregister.</param>
        public void UnregisterCreature(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.creatureMap.TryRemove(creature.Id, out _);
        }

        /// <summary>
        /// Looks for a single creature with the id.
        /// </summary>
        /// <param name="creatureId">The creature id for which to look.</param>
        /// <returns>The creature instance, if found, and null otherwise.</returns>
        public ICreature FindCreatureById(Guid creatureId)
        {
            try
            {
                return this.creatureMap[creatureId];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all creatures known to this manager.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        public IEnumerable<ICreature> FindAllCreatures()
        {
            return this.creatureMap.Values;
        }
    }
}
