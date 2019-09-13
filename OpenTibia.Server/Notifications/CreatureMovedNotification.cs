// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class CreatureMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotification"/> class.
        /// </summary>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureMovedNotification(Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureMovedNotificationArguments arguments)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureMovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
        {
            if (!(Game.Instance.GetCreatureWithId(this.PlayerId) is IPlayer player))
            {
                return;
            }

            var creature = Game.Instance.GetCreatureWithId(this.Arguments.CreatureId);

            if (this.Arguments.CreatureId == this.PlayerId)
            {
                if (this.Arguments.WasTeleport)
                {
                    // TODO: reviset this. Not sure if it should be >= 10.
                    if (this.Arguments.OldStackPosition < 10)
                    {
                        this.Packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                    }

                    this.Packets.Add(new MapDescriptionPacket(this.Arguments.NewLocation, Game.Instance.GetMapDescriptionAt(player, this.Arguments.NewLocation)));
                }
                else
                {
                    if (this.Arguments.OldLocation.Z == 7 && this.Arguments.NewLocation.Z > 7)
                    {
                        if (this.Arguments.OldStackPosition < 10)
                        {
                            this.Packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                        }
                    }
                    else
                    {
                        this.Packets.Add(new CreatureMovedPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition, this.Arguments.NewLocation));
                    }

                    // floor change down
                    if (this.Arguments.NewLocation.Z > this.Arguments.OldLocation.Z)
                    {
                        // going from surface to underground
                        if (this.Arguments.NewLocation.Z == 8)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(
                                OutgoingGamePacketType.FloorChangeDown,
                                Game.Instance.GetMapFloorsDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y - 6), this.Arguments.NewLocation.Z, (byte)(this.Arguments.NewLocation.Z + 2), 18, 14, -1)));
                        }

                        // going further down
                        else if (this.Arguments.NewLocation.Z > this.Arguments.OldLocation.Z && this.Arguments.NewLocation.Z > 8 && this.Arguments.NewLocation.Z < 14)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(
                                OutgoingGamePacketType.FloorChangeDown,
                                Game.Instance.GetMapFloorsDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y - 6), (byte)(this.Arguments.NewLocation.Z + 2), (byte)(this.Arguments.NewLocation.Z + 2), 18, 14, -3)));
                        }
                        else
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(
                                OutgoingGamePacketType.FloorChangeDown,
                                new byte[0]));
                        }

                        // moving down a floor makes us out of sync, include east and south
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceEast,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.OldLocation.X + 9), (ushort)(this.Arguments.OldLocation.Y - 7), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 1, 14)));

                        // south
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceSouth,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y + 7), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 18, 1)));
                    }

                    // floor change up
                    else if (this.Arguments.NewLocation.Z < this.Arguments.OldLocation.Z)
                    {
                        // going to surface
                        if (this.Arguments.NewLocation.Z == 7)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(
                                OutgoingGamePacketType.FloorChangeUp,
                                Game.Instance.GetMapFloorsDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y - 6), 5, 0, 18, 14, 3)));
                        }

                        // underground, going one floor up (still underground)
                        else if (this.Arguments.NewLocation.Z > 7)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(
                                OutgoingGamePacketType.FloorChangeUp,
                                Game.Instance.GetMapFloorsDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y - 6), (byte)(this.Arguments.OldLocation.Z - 3), (byte)(this.Arguments.OldLocation.Z - 3), 18, 14, 3)));
                        }
                        else
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(
                                OutgoingGamePacketType.FloorChangeUp,
                                new byte[0]));
                        }

                        // moving up a floor up makes us out of sync, include west and north
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceWest,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y - 5), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 1, 14)));

                        // north
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceNorth,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.OldLocation.Y - 6), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 18, 1)));
                    }

                    if (this.Arguments.OldLocation.Y > this.Arguments.NewLocation.Y)
                    {
                        // north, for old x
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceNorth,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.NewLocation.Y - 6), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 18, 1)));
                    }
                    else if (this.Arguments.OldLocation.Y < this.Arguments.NewLocation.Y)
                    {
                        // south, for old x
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceSouth,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.OldLocation.X - 8), (ushort)(this.Arguments.NewLocation.Y + 7), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 18, 1)));
                    }

                    if (this.Arguments.OldLocation.X < this.Arguments.NewLocation.X)
                    {
                        // east, [with new y]
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceEast,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.NewLocation.X + 9), (ushort)(this.Arguments.NewLocation.Y - 6), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 1, 14)));
                    }
                    else if (this.Arguments.OldLocation.X > this.Arguments.NewLocation.X)
                    {
                        // west, [with new y]
                        this.Packets.Add(new MapPartialDescriptionPacket(
                            OutgoingGamePacketType.MapSliceWest,
                            Game.Instance.GetMapDescription(player, (ushort)(this.Arguments.NewLocation.X - 8), (ushort)(this.Arguments.NewLocation.Y - 6), this.Arguments.NewLocation.Z, this.Arguments.NewLocation.IsUnderground, 1, 14)));
                    }
                }
            }
            else if (player.CanSee(this.Arguments.OldLocation) && player.CanSee(this.Arguments.NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (this.Arguments.WasTeleport || (this.Arguments.OldLocation.Z == 7 && this.Arguments.NewLocation.Z > 7) || this.Arguments.OldStackPosition > 9)
                    {
                        if (this.Arguments.OldStackPosition < 10)
                        {
                            this.Packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                        }

                        this.Packets.Add(new AddCreaturePacket(creature, player.KnowsCreatureWithId(this.Arguments.CreatureId), player.ChooseToRemoveFromKnownSet()));
                    }
                    else
                    {
                        this.Packets.Add(new CreatureMovedPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition, this.Arguments.NewLocation));
                    }
                }
            }
            else if (player.CanSee(this.Arguments.OldLocation) && player.CanSee(creature))
            {
                if (this.Arguments.OldStackPosition < 10)
                {
                    this.Packets.Add(new RemoveAtStackposPacket(this.Arguments.OldLocation, this.Arguments.OldStackPosition));
                }
            }
            else if (player.CanSee(this.Arguments.NewLocation) && player.CanSee(creature))
            {
                this.Packets.Add(new AddCreaturePacket(creature, player.KnowsCreatureWithId(this.Arguments.CreatureId), player.ChooseToRemoveFromKnownSet()));
            }

            // if (this.WasTeleport)
            // {
            //    this.ResponsePackets.Add(new MagicEffectPacket()
            //    {
            //        Location = this.Arguments.NewLocation,
            //        Effect = Effect_t.BubbleBlue
            //    });
            // }
        }
    }
}