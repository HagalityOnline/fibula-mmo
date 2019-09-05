// <copyright file="UseItemPlayerAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Actions
{
    using System;
    using System.Linq;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Events;

    internal class UseItemPlayerAction : BasePlayerAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseItemPlayerAction"/> class.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="itemUsePacket"></param>
        /// <param name="retryLocation"></param>
        public UseItemPlayerAction(IPlayer player, ItemUsePacket itemUsePacket, Location retryLocation)
            : base(player, itemUsePacket, retryLocation)
        {
        }

        protected override void InternalPerform()
        {
            IThing thingToUse = null;

            if (!(this.Packet is ItemUsePacket itemUsePacket))
            {
                return;
            }

            switch (itemUsePacket.FromLocation.Type)
            {
                case LocationType.Ground:
                    thingToUse = Game.Instance.GetTileAt(itemUsePacket.FromLocation)?.GetThingAtStackPosition(itemUsePacket.FromStackPos);
                    break;
                case LocationType.Container:
                    var fromContainer = this.Player.GetContainer(itemUsePacket.FromLocation.Container);
                    try
                    {
                        thingToUse = fromContainer.Content[fromContainer.Content.Count - itemUsePacket.FromStackPos - 1];
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                    } // Happens when the content list does not contain the thing.
                    break;
                case LocationType.Slot:
                    try
                    {
                        thingToUse = this.Player.Inventory[Convert.ToByte(itemUsePacket.FromLocation.Slot)];
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (thingToUse == null)
            {
                // No thing to use found, exit here.
                return;
            }

            var thingAsItem = thingToUse as IItem;

            if (thingAsItem != null && thingAsItem.ChangesOnUse)
            {
                Functions.Change(ref thingToUse, thingAsItem.ChangeOnUseTo, 0);
            }
            else if (thingAsItem != null && thingAsItem.IsContainer && thingAsItem is IContainer thingAsContainer)
            {
                var openContainerId = this.Player.GetContainerId(thingAsContainer);

                if (openContainerId < 0)
                {
                    // PlayerId doesn't have this container open.
                    switch (itemUsePacket.FromLocation.Type)
                    {
                        case LocationType.Ground:
                            this.Player.OpenContainer(thingAsContainer);
                            break;
                        case LocationType.Container:
                            this.Player.OpenContainerAt(thingAsContainer, itemUsePacket.Index);
                            break;
                        case LocationType.Slot:
                            this.Player.OpenContainerAt(thingAsContainer, itemUsePacket.Index);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    thingAsContainer.OnThingChanged += this.Player.CheckInventoryContainerProximity;

                    this.ResponsePackets.Add(new ContainerOpenPacket
                    {
                        ContainerId = (byte)thingAsContainer.GetIdFor(this.Player.Id),
                        ClientItemId = thingAsItem.ThingId,
                        HasParent = thingAsContainer.Parent != null,
                        Name = thingAsItem.Type.Name,
                        Volume = thingAsContainer.Volume,
                        Contents = thingAsContainer.Content,
                    });
                }
                else
                {
                    // Close it.
                    this.Player.CloseContainerWithId((byte)openContainerId);
                    thingAsContainer.OnThingChanged -= this.Player.CheckInventoryContainerProximity;

                    this.ResponsePackets.Add(new ContainerClosePacket
                    {
                        ContainerId = (byte)openContainerId,
                    });
                }
            }
            else
            {
                var useEvents = Game.Instance.EventsCatalog[ItemEventType.Use].Cast<UseItemEvent>();

                var candidate = useEvents.FirstOrDefault(e => e.ItemToUseId == itemUsePacket.ClientId && e.Setup(thingToUse, null, this.Player) && e.CanBeExecuted);

                if (candidate != null)
                {
                    // Execute all actions.
                    candidate.Execute();
                }
            }
        }
    }
}