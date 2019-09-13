// -----------------------------------------------------------------
// <copyright file="ScriptFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Monsters;
    using OpenTibia.Server.Movement;
    using OpenTibia.Server.Notifications;
    using OpenTibia.Server.Scripting;

    public class ScriptFactory : IScriptFactory
    {
        public const string ThingOneIdentifier = "Obj1";
        public const string ThingTwoIdentifier = "Obj2";
        public const string CurrentUserIdentifier = "User";

        public const string NameShorthand = "%N";

        private static TimeSpan defaultDelayForFunctions = TimeSpan.FromMilliseconds(1);

        public ScriptFactory(
            IGame gameInstance,
            ICreatureFinder creatureFinder,
            INotificationFactory notificationFactory,
            ITileAccessor tileAccessor)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));
            creatureFinder.ThrowIfNull(nameof(creatureFinder));
            notificationFactory.ThrowIfNull(nameof(notificationFactory));
            tileAccessor.ThrowIfNull(nameof(tileAccessor));

            this.Game = gameInstance;
            this.CreatureFinder = creatureFinder;
            this.NotificationFactory = notificationFactory;
            this.TileAccessor = tileAccessor;
        }

        public IGame Game { get; }

        public ICreatureFinder CreatureFinder { get; }

        public INotificationFactory NotificationFactory { get; }

        public ITileAccessor TileAccessor { get; }

        public bool InvokeCondition(IThing obj1, IThing obj2, IPlayer user, string methodName, params object[] parameters)
        {
            methodName.ThrowIfNullOrWhiteSpace(nameof(methodName));

            var negateCondition = methodName.StartsWith("!");

            methodName = methodName.TrimStart('!');

            var methodInfo = this.GetType().GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(this.GetType().Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>();

                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (ThingOneIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj1);
                    }
                    else if (ThingTwoIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj2);
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(user);
                    }
                    else
                    {
                        parametersForInvocation.Add((parameters[i] as string).ConvertStringToNewType(methodParameters[i].ParameterType));
                    }
                }

                var result = (bool)methodInfo.Invoke(this, parametersForInvocation.ToArray());

                return negateCondition ? !result : result;
            }
            catch (Exception ex)
            {
                // TODO: proper logging
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return false;
        }

        public void InvokeAction(ref IThing obj1, ref IThing obj2, ref IPlayer user, string methodName, params object[] parameters)
        {
            methodName.ThrowIfNullOrWhiteSpace(nameof(methodName));

            var methodInfo = this.GetType().GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(this.GetType().Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>();

                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (ThingOneIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj1);
                    }
                    else if (ThingTwoIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(obj2);
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        parametersForInvocation.Add(user);
                    }
                    else
                    {
                        var convertedValue = (parameters[i] as string).ConvertStringToNewType(methodParameters[i].ParameterType);

                        parametersForInvocation.Add(convertedValue);
                    }
                }

                var paramsArray = parametersForInvocation.ToArray();

                methodInfo.Invoke(this, paramsArray);

                // update references to special parameters.
                for (var i = 0; i < methodParameters.Length; i++)
                {
                    if (ThingOneIdentifier.Equals(parameters[i] as string))
                    {
                        obj1 = paramsArray[i] as IThing;
                    }
                    else if (ThingTwoIdentifier.Equals(parameters[i] as string))
                    {
                        obj2 = paramsArray[i] as IThing;
                    }
                    else if (CurrentUserIdentifier.Equals(parameters[i] as string))
                    {
                        user = paramsArray[i] as IPlayer;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public bool CountObjects(IThing thingAt, string comparer, ushort value)
        {
            if (thingAt?.Tile == null || string.IsNullOrWhiteSpace(comparer))
            {
                return false;
            }

            var count = thingAt.Tile.Ground == null ? 0 : 1;

            count += thingAt.Tile.DownItems.Count();

            switch (comparer.Trim())
            {
                case "=":
                case "==":
                    return count == value;
                case ">=":
                    return count >= value;
                case "<=":
                    return count <= value;
                case ">":
                    return count > value;
                case "<":
                    return count < value;
            }

            return false;
        }

        public bool IsCreature(IThing thing)
        {
            return thing is ICreature;
        }

        public bool IsType(IThing thing, ushort typeId)
        {
            return thing is IItem item && item.Type.TypeId == typeId;
        }

        public bool IsPosition(IThing thing, Location location)
        {
            return thing != null && thing.Location == location;
        }

        public bool IsPlayer(IThing thing)
        {
            return thing is IPlayer;
        }

        public bool IsObjectThere(Location location, ushort typeId)
        {
            var targetTile = this.TileAccessor.GetTileAt(location);

            return targetTile?.BruteFindItemWithId(typeId) != null;
        }

        public bool HasRight(IPlayer user, string rightStr)
        {
            return true; // TODO: implement.
        }

        public bool MayLogout(IPlayer player)
        {
            return player.IsLogoutAllowed;
        }

        public bool HasFlag(IThing itemThing, string flagStr)
        {
            if (!(itemThing is IItem))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(flagStr))
            {
                return true;
            }

            return Enum.TryParse(flagStr, out ItemFlag parsedFlag) && ((IItem)itemThing).Type.Flags.Contains(parsedFlag);
        }

        public bool HasProfession(IThing thing, byte profesionId)
        {
            // TODO: implement professions.
            return thing != null && thing is IPlayer && false;
        }

        public bool HasInstanceAttribute(IThing thing, string attributeStr, string comparer, ushort value)
        {
            if (thing == null || string.IsNullOrWhiteSpace(attributeStr) || string.IsNullOrWhiteSpace(comparer) || !(thing is IItem thingAsItem))
            {
                return false;
            }

            if (!Enum.TryParse(attributeStr, out ItemAttribute actualAttribute))
            {
                return false;
            }

            if (!thingAsItem.Attributes.ContainsKey(actualAttribute))
            {
                return false;
            }

            switch (comparer.Trim())
            {
                case "=":
                case "==":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) == value;
                case ">=":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) >= value;
                case "<=":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) <= value;
                case ">":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) > value;
                case "<":
                    return Convert.ToUInt16(thingAsItem.Attributes[actualAttribute]) < value;
            }

            return false;
        }

        public bool IsHouse(IThing thing)
        {
            return thing?.Tile != null && thing.Tile.IsHouse;
        }

        public bool IsHouseOwner(IThing thing, IPlayer user)
        {
            // TODO: implement house ownership.
            return this.IsHouse(thing); // && thing.Tile.House.Owner == user.Name;
        }

        public bool Random(byte value)
        {
            // TODO: is this really bound to 100? or do we need more precision.
            return new Random().Next(100) <= value;
        }

        public void Create(IThing atThing, ushort itemId, byte unknown)
        {
            IThing item = ItemFactory.Create(itemId);

            var targetTile = atThing.Tile;

            if (item == null || targetTile == null)
            {
                return;
            }

            this.Game.AssetManagement.AddThingToTile(ref item, targetTile);
        }

        public void CreateOnMap(Location location, ushort itemId, byte unknown)
        {
            IThing item = ItemFactory.Create(itemId);

            if (item == null)
            {
                return;
            }

            var targetTile = this.TileAccessor.GetTileAt(location);

            if (targetTile == null)
            {
                return;
            }

            targetTile.AddThing(ref item);

            this.Game.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, gameInstance.GetMapTileDescription(conn.PlayerId, location)), location);
        }

        public void ChangeOnMap(Location location, ushort fromItemId, ushort toItemId, byte unknown)
        {
            var targetTile = this.TileAccessor.GetTileAt(location);
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(fromItemId);
            targetTile.AddThing(ref newThing);

            this.Game.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, gameInstance.GetMapTileDescription(conn.PlayerId, location)), targetTile.Location);
        }

        public void Effect(IThing thing, byte effectByte)
        {
            if (thing == null)
            {
                return;
            }

            if (effectByte == 0 || effectByte > (byte)AnimatedEffect.SoundWhite)
            {
                Console.WriteLine($"Invalid effect {effectByte} called, ignored.");
                return;
            }

            // TODO: fix this
            //gameInstance.NotifySpectatingPlayers(
            //    this.NotificationFactory.Create(
            //        NotificationType.Generic,
            //        new GenericNotificationArguments(
            //            new[] { new MagicEffectPacket(thing.Location, (AnimatedEffect)effectByte) },
            //            playerIds: ),
            //    thing.Location));
        }

        public void EffectOnMap(Location location, byte effectByte)
        {
            if (effectByte == 0 || effectByte > (byte)AnimatedEffect.SoundWhite)
            {
                Console.WriteLine($"Invalid effect {effectByte} called on map, ignored.");
                return;
            }

            this.Game.NotifySpectatingPlayers(
                conn => new GenericNotification(
                    conn,
                    new MagicEffectPacket { Location = location, Effect = (AnimatedEffect)effectByte }),
                location);
        }

        public void Delete(IThing thing)
        {
            var targetTile = thing?.Tile;

            if (thing == null || targetTile == null)
            {
                return;
            }

            var toRemove = thing;
            targetTile.RemoveThing(ref toRemove, thing.Count);

            this.Game.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void DeleteOnMap(Location location, ushort itemId)
        {
            var targetTile = this.TileAccessor.GetTileAt(location);

            if (targetTile == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(itemId);

            this.Game.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, gameInstance.GetMapTileDescription(conn.PlayerId, location)), targetTile.Location);
        }

        public void MonsterOnMap(Location location, ushort monsterId)
        {
            var monster = MonsterFactory.Create(monsterId);

            if (monster != null)
            {
                IThing monsterAsThing = monster;
                var tile = this.TileAccessor.GetTileAt(location);

                if (tile == null)
                {
                    Console.WriteLine($"Unable to place monster at {location}, no tile there.");
                    return;
                }

                // place the monster.
                tile.AddThing(ref monsterAsThing);

                if (!this.Game.Creatures.TryAdd(monster.Id, monster))
                {
                    Console.WriteLine($"WARNING: Failed to add {monster.Article} {monster.Name} to the global dictionary.");
                }

                var creatureAddedNotification = this.NotificationFactory.Create(
                    NotificationType.CreatureAdded,
                    new CreatureAddedNotificationArguments(monster, AnimatedEffect.BubbleBlue));

                this.Game.ScheduleEvent(creatureAddedNotification);

                this.Game.NotifySpectatingPlayers(conn => new CreatureAddedNotification(conn, monster, AnimatedEffect.BubbleBlue), monster.Location);
            }
        }

        public void Change(ref IThing thing, ushort toItemId, byte unknown)
        {
            if (thing == null)
            {
                return;
            }

            var targetTile = thing.Tile;
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            var toRemove = thing;
            targetTile.RemoveThing(ref toRemove);
            targetTile.AddThing(ref newThing);

            thing = newThing;

            this.Game.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void ChangeRel(IThing fromThing, Location locationOffset, ushort fromItemId, ushort toItemId, byte unknown)
        {
            if (fromThing == null)
            {
                return;
            }

            var targetTile = this.TileAccessor.GetTileAt(fromThing.Location + locationOffset);
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(fromItemId);
            targetTile.AddThing(ref newThing);

            this.Game.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void Damage(IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue)
        {
            // TODO: implement correctly when combat is...

            if (!(damagedThing is ICreature damagedCreature))
            {
                return;
            }

            switch (damageSourceType)
            {
                default: // physical
                    break;
                case 2: // magic? or mana?
                    break;
                case 4: // fire instant
                    this.Effect(damagedThing, (byte)AnimatedEffect.Flame);
                    break;
                case 8: // energy instant
                    this.Effect(damagedThing, (byte)AnimatedEffect.DamageEnergy);
                    break;
                case 16: // poison instant?
                    this.Effect(damagedThing, (byte)AnimatedEffect.RingsGreen);
                    break;
                case 32: // poison over time (poisoned condition)
                    break;
                case 64: // fire over time (burned condition)
                    break;
                case 128: // energy over time (electrified condition)
                    break;
            }
        }

        public void Logout(IPlayer user)
        {
            this.Game.AttemptLogout(user); // TODO: force?
        }

        public void Move(IThing thingToMove, Location targetLocation)
        {
            if (thingToMove == null)
            {
                return;
            }

            if (thingToMove is ICreature thingAsCreature)
            {
                this.Game.ScheduleEvent(new CreatureMovementOnMap(0, thingAsCreature, thingToMove.Location, targetLocation), defaultDelayForFunctions);
            }
            else if (thingToMove is IItem thingAsItem)
            {
                this.Game.ScheduleEvent(new OnMapMovementEvent(0, thingAsItem, thingToMove.Location, thingToMove.Tile.GetStackPosition(thingToMove), targetLocation, thingAsItem.Count), defaultDelayForFunctions);
            }
        }

        public void MoveRel(ICreature user, IThing objectUsed, Location locationOffset)
        {
            this.Game.ScheduleEvent(new OnMapMovementEvent(0, user, user.Location, user.Tile.GetStackPosition(user), objectUsed.Location + locationOffset, 1, true), defaultDelayForFunctions);
        }

        public void MoveTop(IThing fromThing, Location targetLocation)
        {
            if (fromThing == null)
            {
                return;
            }

            // Move all down items and creatures on tile.
            foreach (var item in fromThing.Tile.DownItems.ToList())
            {
                this.Game.ScheduleEvent(new OnMapMovementEvent(0, item, fromThing.Location, fromThing.Tile.GetStackPosition(item), targetLocation), defaultDelayForFunctions);
            }

            foreach (var creatureId in fromThing.Tile.CreatureIds.ToList())
            {
                var creature = this.CreatureFinder.FindCreatureById(creatureId);

                if (creature != null)
                {
                    this.Game.ScheduleEvent(new CreatureMovementOnMap(0, creature, fromThing.Location, targetLocation, true), defaultDelayForFunctions);
                }
            }
        }

        /// <summary>
        /// Moves the top thing in the stack of the <paramref name="fromThing"/>'s <see cref="Thing.Tile"/> to the relative location off of it.
        /// </summary>
        /// <param name="fromThing">The reference <see cref="Thing"/> to move from.</param>
        /// <param name="locationOffset">The <see cref="Location"/> offset to move to.</param>
        public void MoveTopRel(IThing fromThing, Location locationOffset)
        {
            var targetLocation = fromThing.Location + locationOffset;

            // Move all down items and creatures on tile.
            foreach (var item in fromThing.Tile.DownItems.ToList())
            {
                this.Game.ScheduleEvent(new OnMapMovementEvent(0, item, fromThing.Location, fromThing.Tile.GetStackPosition(fromThing), targetLocation, item.Count), defaultDelayForFunctions);
            }

            foreach (var creatureId in fromThing.Tile.CreatureIds.ToList())
            {
                var creature = this.CreatureFinder.FindCreatureById(creatureId);

                if (creature != null)
                {
                    this.Game.ScheduleEvent(new CreatureMovementOnMap(0, creature, fromThing.Location, targetLocation, true), defaultDelayForFunctions);
                }
            }
        }

        public void MoveTopOnMap(Location fromLocation, ushort itemId, Location toLocation)
        {
            var tile = this.TileAccessor.GetTileAt(fromLocation);

            if (tile == null)
            {
                return;
            }

            this.MoveTop(tile.BruteFindItemWithId(itemId), toLocation);
        }

        public void Text(IThing fromThing, string text, byte textType)
        {
        }

        public void WriteName(IPlayer user, string format, IThing targetThing)
        {
            // TODO: implement.
        }

        public void SetStart(IThing thing, Location location)
        {
            if (!(thing is IPlayer))
            {
            }

            // TODO: implement.
        }
    }
}
