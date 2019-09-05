// <copyright file="CreatureMovedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class CreatureMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureMovedNotification(CreatureMovedNotificationArguments arguments)
            : base(audience, playerId)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureMovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public override void Prepare()
        {
            if (!(Game.Instance.GetCreatureWithId(this.PlayerId) is IPlayer player))
            {
                return;
            }

            var creature = Game.Instance.GetCreatureWithId(this.CreatureId);

            if (this.CreatureId == this.PlayerId)
            {
                if (this.WasTeleport) // TODO: revise; this had a contradicting condition on the source (< 10 vs >= 10)
                {
                    if (this.OldStackPosition < 10)
                    {
                        this.Packets.Add(new RemoveAtStackposPacket
                        {
                            Location = this.OldLocation,
                            Stackpos = this.OldStackPosition,
                        });
                    }

                    this.Packets.Add(new MapDescriptionPacket
                    {
                        Origin = this.NewLocation,
                        DescriptionBytes = Game.Instance.GetMapDescriptionAt(player, this.NewLocation),
                    });
                }
                else
                {
                    if (this.OldLocation.Z == 7 && this.NewLocation.Z > 7)
                    {
                        if (this.OldStackPosition < 10)
                        {
                            this.Packets.Add(new RemoveAtStackposPacket
                            {
                                Location = this.OldLocation,
                                Stackpos = this.OldStackPosition,
                            });
                        }
                    }
                    else
                    {
                        this.Packets.Add(new CreatureMovedPacket
                        {
                            FromLocation = this.OldLocation,
                            FromStackpos = this.OldStackPosition,
                            ToLocation = this.NewLocation,
                        });
                    }

                    // floor change down
                    if (this.NewLocation.Z > this.OldLocation.Z)
                    {
                        // going from surface to underground
                        if (this.NewLocation.Z == 8)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeDown)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), this.NewLocation.Z, (byte)(this.NewLocation.Z + 2), 18, 14, -1),
                            });
                        }

                        // going further down
                        else if (this.NewLocation.Z > this.OldLocation.Z && this.NewLocation.Z > 8 && this.NewLocation.Z < 14)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeDown)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), (byte)(this.NewLocation.Z + 2), (byte)(this.NewLocation.Z + 2), 18, 14, -3),
                            });
                        }
                        else
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeDown)
                            {
                                DescriptionBytes = new byte[0], // no description needed.
                            });
                        }

                        // moving down a floor makes us out of sync, include east and south
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceEast)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X + 9), (ushort)(this.OldLocation.Y - 7), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14),
                        });

                        // south
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceSouth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y + 7), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1),
                        });
                    }

                    // floor change up
                    else if (this.NewLocation.Z < this.OldLocation.Z)
                    {
                        // going to surface
                        if (this.NewLocation.Z == 7)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeUp)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), 5, 0, 18, 14, 3),
                            });
                        }

                        // underground, going one floor up (still underground)
                        else if (this.NewLocation.Z > 7)
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeUp)
                            {
                                DescriptionBytes = Game.Instance.GetMapFloorsDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), (byte)(this.OldLocation.Z - 3), (byte)(this.OldLocation.Z - 3), 18, 14, 3),
                            });
                        }
                        else
                        {
                            this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.FloorChangeUp)
                            {
                                DescriptionBytes = new byte[0], // no description needed.
                            });
                        }

                        // moving up a floor up makes us out of sync, include west and north
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceWest)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 5), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14),
                        });

                        // north
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceNorth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.OldLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1),
                        });
                    }

                    if (this.OldLocation.Y > this.NewLocation.Y)
                    {
                        // north, for old x
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceNorth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.NewLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1),
                        });
                    }
                    else if (this.OldLocation.Y < this.NewLocation.Y)
                    {
                        // south, for old x
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceSouth)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.OldLocation.X - 8), (ushort)(this.NewLocation.Y + 7), this.NewLocation.Z, this.NewLocation.IsUnderground, 18, 1),
                        });
                    }

                    if (this.OldLocation.X < this.NewLocation.X)
                    {
                        // east, [with new y]
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceEast)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.NewLocation.X + 9), (ushort)(this.NewLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14),
                        });
                    }
                    else if (this.OldLocation.X > this.NewLocation.X)
                    {
                        // west, [with new y]
                        this.Packets.Add(new MapPartialDescriptionPacket(OutgoingGamePacketType.MapSliceWest)
                        {
                            DescriptionBytes = Game.Instance.GetMapDescription(player, (ushort)(this.NewLocation.X - 8), (ushort)(this.NewLocation.Y - 6), this.NewLocation.Z, this.NewLocation.IsUnderground, 1, 14),
                        });
                    }
                }
            }
            else if (player.CanSee(this.OldLocation) && player.CanSee(this.NewLocation))
            {
                if (player.CanSee(creature))
                {
                    if (this.WasTeleport || (this.OldLocation.Z == 7 && this.NewLocation.Z > 7) || this.OldStackPosition > 9)
                    {
                        if (this.OldStackPosition < 10)
                        {
                            this.Packets.Add(new RemoveAtStackposPacket
                            {
                                Location = this.OldLocation,
                                Stackpos = this.OldStackPosition,
                            });
                        }

                        this.Packets.Add(new AddCreaturePacket
                        {
                            Location = this.NewLocation,
                            Creature = creature,
                            AsKnown = player.KnowsCreatureWithId(this.CreatureId),
                            RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet(), // chooses a victim if neeeded.
                        });
                    }
                    else
                    {
                        this.Packets.Add(new CreatureMovedPacket
                        {
                            FromLocation = this.OldLocation,
                            FromStackpos = this.OldStackPosition,
                            ToLocation = this.NewLocation,
                        });
                    }
                }
            }
            else if (player.CanSee(this.OldLocation) && player.CanSee(creature))
            {
                if (this.OldStackPosition < 10)
                {
                    this.Packets.Add(new RemoveAtStackposPacket
                    {
                        Location = this.OldLocation,
                        Stackpos = this.OldStackPosition,
                    });
                }
            }
            else if (player.CanSee(this.NewLocation) && player.CanSee(creature))
            {
                this.Packets.Add(new AddCreaturePacket
                {
                    Location = this.NewLocation,
                    Creature = creature,
                    AsKnown = player.KnowsCreatureWithId(this.CreatureId),
                    RemoveThisCreatureId = player.ChooseToRemoveFromKnownSet(), // chooses a victim if neeeded.
                });
            }

            // if (this.WasTeleport)
            // {
            //    this.ResponsePackets.Add(new MagicEffectPacket()
            //    {
            //        Location = this.NewLocation,
            //        Effect = Effect_t.BubbleBlue
            //    });
            // }
        }
    }
}