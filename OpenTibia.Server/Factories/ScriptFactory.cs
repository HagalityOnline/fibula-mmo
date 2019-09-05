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

        public bool InvokeCondition(IGame gameInstance, IThing obj1, IThing obj2, IPlayer user, string methodName, params object[] parameters)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));
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

                var parametersForInvocation = new List<object>()
                {
                    gameInstance,
                };

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

        public void InvokeAction(IGame gameInstance, ref IThing obj1, ref IThing obj2, ref IPlayer user, string methodName, params object[] parameters)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));
            methodName.ThrowIfNullOrWhiteSpace(nameof(methodName));

            var methodInfo = this.GetType().GetMethod(methodName);

            try
            {
                if (methodInfo == null)
                {
                    throw new MissingMethodException(this.GetType().Name, methodName);
                }

                var methodParameters = methodInfo.GetParameters();

                var parametersForInvocation = new List<object>()
                {
                    gameInstance,
                };

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

        public bool IsObjectThere(IGame gameInstance, Location location, ushort typeId)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var targetTile = gameInstance.GetTileAt(location);

            return targetTile?.BruteFindItemWithId(typeId) != null;
        }

        public bool HasRight(IPlayer user, string rightStr)
        {
            return true; // TODO: implement.
        }

        public bool MayLogout(IPlayer user)
        {
            return user.CanLogout;
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
            return thing != null && thing is IPlayer && false; // TODO: implement professions.
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
            return this.IsHouse(thing); // && thing.Tile.House.Owner == user.Name;
        }

        public bool Random(byte value)
        {
            return new Random().Next(100) <= value;
        }

        public void Create(IGame gameInstance, IThing atThing, ushort itemId, byte unknown)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            IThing item = ItemFactory.Create(itemId);
            var targetTile = atThing.Tile;

            if (item == null || targetTile == null)
            {
                return;
            }

            targetTile.AddThing(ref item);

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void CreateOnMap(IGame gameInstance, Location location, ushort itemId, byte unknown)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            IThing item = ItemFactory.Create(itemId);

            if (item == null)
            {
                return;
            }

            var targetTile = gameInstance.GetTileAt(location);

            if (targetTile == null)
            {
                return;
            }

            targetTile.AddThing(ref item);

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, gameInstance.GetMapTileDescription(conn.PlayerId, location)), location);
        }

        public void ChangeOnMap(IGame gameInstance, Location location, ushort fromItemId, ushort toItemId, byte unknown)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var targetTile = gameInstance.GetTileAt(location);
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(fromItemId);
            targetTile.AddThing(ref newThing);

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, gameInstance.GetMapTileDescription(conn.PlayerId, location)), targetTile.Location);
        }

        public void Effect(IGame gameInstance, IThing thing, byte effectByte)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            if (thing == null)
            {
                return;
            }

            if (effectByte == 0 || effectByte > (byte)AnimatedEffect.SoundWhite)
            {
                Console.WriteLine($"Invalid effect {effectByte} called, ignored.");
                return;
            }

            gameInstance.NotifySpectatingPlayers(
                conn => new GenericNotification(
                    conn,
                    new MagicEffectPacket { Location = thing.Location, Effect = (AnimatedEffect)effectByte }),
                thing.Location);
        }

        public void EffectOnMap(IGame gameInstance, Location location, byte effectByte)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            if (effectByte == 0 || effectByte > (byte)AnimatedEffect.SoundWhite)
            {
                Console.WriteLine($"Invalid effect {effectByte} called on map, ignored.");
                return;
            }

            gameInstance.NotifySpectatingPlayers(
                conn => new GenericNotification(
                    conn,
                    new MagicEffectPacket { Location = location, Effect = (AnimatedEffect)effectByte }),
                location);
        }

        public void Delete(IGame gameInstance, IThing thing)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var targetTile = thing?.Tile;

            if (thing == null || targetTile == null)
            {
                return;
            }

            var toRemove = thing;
            targetTile.RemoveThing(ref toRemove, thing.Count);

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void DeleteOnMap(IGame gameInstance, Location location, ushort itemId)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var targetTile = gameInstance.GetTileAt(location);

            if (targetTile == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(itemId);

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, location, gameInstance.GetMapTileDescription(conn.PlayerId, location)), targetTile.Location);
        }

        public void MonsterOnMap(IGame gameInstance, Location location, ushort monsterId)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var monster = MonsterFactory.Create(monsterId);

            if (monster != null)
            {
                IThing monsterAsThing = monster;
                var tile = gameInstance.GetTileAt(location);

                if (tile == null)
                {
                    Console.WriteLine($"Unable to place monster at {location}, no tile there.");
                    return;
                }

                // place the monster.
                tile.AddThing(ref monsterAsThing);

                if (!gameInstance.Creatures.TryAdd(monster.Id, monster))
                {
                    Console.WriteLine($"WARNING: Failed to add {monster.Article} {monster.Name} to the global dictionary.");
                }

                gameInstance.NotifySpectatingPlayers(conn => new CreatureAddedNotification(conn, monster, AnimatedEffect.BubbleBlue), monster.Location);
            }
        }

        public void Change(IGame gameInstance, ref IThing thing, ushort toItemId, byte unknown)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

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

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void ChangeRel(IGame gameInstance, IThing fromThing, Location locationOffset, ushort fromItemId, ushort toItemId, byte unknown)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            if (fromThing == null)
            {
                return;
            }

            var targetTile = gameInstance.GetTileAt(fromThing.Location + locationOffset);
            IThing newThing = ItemFactory.Create(toItemId);

            if (targetTile == null || newThing == null)
            {
                return;
            }

            targetTile.BruteRemoveItemWithId(fromItemId);
            targetTile.AddThing(ref newThing);

            gameInstance.NotifySpectatingPlayers(conn => new TileUpdatedNotification(conn, targetTile.Location, gameInstance.GetMapTileDescription(conn.PlayerId, targetTile.Location)), targetTile.Location);
        }

        public void Damage(IGame gameInstance, IThing damagingThing, IThing damagedThing, byte damageSourceType, ushort damageValue)
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
                    this.Effect(gameInstance, damagedThing, (byte)AnimatedEffect.Flame);
                    break;
                case 8: // energy instant
                    this.Effect(gameInstance, damagedThing, (byte)AnimatedEffect.DamageEnergy);
                    break;
                case 16: // poison instant?
                    this.Effect(gameInstance, damagedThing, (byte)AnimatedEffect.RingsGreen);
                    break;
                case 32: // poison over time (poisoned condition)
                    break;
                case 64: // fire over time (burned condition)
                    break;
                case 128: // energy over time (electrified condition)
                    break;
            }
        }

        public void Logout(IGame gameInstance, IPlayer user)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            gameInstance.AttemptLogout(user); // TODO: force?
        }

        public void Move(IGame gameInstance, IThing thingToMove, Location targetLocation)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            if (thingToMove == null)
            {
                return;
            }

            if (thingToMove is ICreature thingAsCreature)
            {
                gameInstance.ScheduleEvent(new CreatureMovementOnMap(0, thingAsCreature, thingToMove.Location, targetLocation), defaultDelayForFunctions);
            }
            else if (thingToMove is IItem thingAsItem)
            {
                gameInstance.ScheduleEvent(new OnMapMovementEvent(0, thingAsItem, thingToMove.Location, thingToMove.Tile.GetStackPosition(thingToMove), targetLocation, thingAsItem.Count), defaultDelayForFunctions);
            }
        }

        public void MoveRel(IGame gameInstance, ICreature user, IThing objectUsed, Location locationOffset)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            gameInstance.ScheduleEvent(new OnMapMovementEvent(0, user, user.Location, user.Tile.GetStackPosition(user), objectUsed.Location + locationOffset, 1, true), defaultDelayForFunctions);
        }

        public void MoveTop(IGame gameInstance, IThing fromThing, Location targetLocation)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            if (fromThing == null)
            {
                return;
            }

            // Move all down items and creatures on tile.
            foreach (var item in fromThing.Tile.DownItems.ToList())
            {
                gameInstance.ScheduleEvent(new OnMapMovementEvent(0, item, fromThing.Location, fromThing.Tile.GetStackPosition(item), targetLocation), defaultDelayForFunctions);
            }

            foreach (var creatureId in fromThing.Tile.CreatureIds.ToList())
            {
                gameInstance.ScheduleEvent(new CreatureMovementOnMap(0, gameInstance.GetCreatureWithId(creatureId), fromThing.Location, targetLocation, true), defaultDelayForFunctions);
            }
        }

        /// <summary>
        /// Moves the top thing in the stack of the <paramref name="fromThing"/>'s <see cref="Thing.Tile"/> to the relative location off of it.
        /// </summary>
        /// <param name="fromThing">The reference <see cref="Thing"/> to move from.</param>
        /// <param name="locationOffset">The <see cref="Location"/> offset to move to.</param>
        public void MoveTopRel(IGame gameInstance, IThing fromThing, Location locationOffset)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var targetLocation = fromThing.Location + locationOffset;

            // Move all down items and creatures on tile.
            foreach (var item in fromThing.Tile.DownItems.ToList())
            {
                gameInstance.ScheduleEvent(new OnMapMovementEvent(0, item, fromThing.Location, fromThing.Tile.GetStackPosition(fromThing), targetLocation, item.Count), defaultDelayForFunctions);
            }

            foreach (var creatureId in fromThing.Tile.CreatureIds.ToList())
            {
                gameInstance.ScheduleEvent(new CreatureMovementOnMap(0, gameInstance.GetCreatureWithId(creatureId), fromThing.Location, targetLocation, true), defaultDelayForFunctions);
            }
        }

        public void MoveTopOnMap(IGame gameInstance, Location fromLocation, ushort itemId, Location toLocation)
        {
            gameInstance.ThrowIfNull(nameof(gameInstance));

            var tile = gameInstance.GetTileAt(fromLocation);

            if (tile == null)
            {
                return;
            }

            this.MoveTop(gameInstance, tile.BruteFindItemWithId(itemId), toLocation);
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
