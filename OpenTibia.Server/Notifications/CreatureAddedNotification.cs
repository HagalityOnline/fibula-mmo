// <copyright file="CreatureAddedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a notification for creature being added in sight to players who are close.
    /// </summary>
    internal class CreatureAddedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureAddedNotification"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureAddedNotification(ICreatureFinder creatureFinder, Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, CreatureAddedNotificationArguments arguments)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureAddedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
        {
            if (this.Arguments.AddedEffect != AnimatedEffect.None)
            {
                this.Packets.Add(new MagicEffectPacket(this.Arguments.Creature.Location, this.Arguments.AddedEffect));
            }
        }

        /// <summary>
        /// Sends the notification using the supplied connection.
        /// </summary>
        protected override void Send()
        {
            IEnumerable<IConnection> connections = null;

            try
            {
                INetworkMessage outboundMessage = new NetworkMessage();

                foreach (var packet in this.Packets)
                {
                    packet.WriteToMessage(outboundMessage);
                }

                connections = this.TargetConnectionsFunction?.Invoke();

                if (connections == null)
                {
                    // TODO: log this?
                    return;
                }

                foreach (var connection in connections)
                {
                    if (!(this.CreatureFinder.FindCreatureById(connection.PlayerId) is IPlayer player))
                    {
                        continue;
                    }

                    var playerTailoredMessage = outboundMessage.Copy();

                    var addCreaturePacket = new AddCreaturePacket(this.Arguments.Creature, player.KnowsCreatureWithId(this.Arguments.Creature.Id), player.ChooseToRemoveFromKnownSet());

                    addCreaturePacket.WriteToMessage(playerTailoredMessage);

                    connection.Send(playerTailoredMessage);
                }
            }
            catch (Exception)
            {
                // TODO: log this.
            }
        }
    }
}