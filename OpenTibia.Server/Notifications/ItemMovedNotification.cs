// <copyright file="ItemMovedNotification.cs" company="2Dudes">
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
    using OpenTibia.Communications.Packets.Outgoing;

    internal class ItemMovedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMovedNotification"/> class.
        /// </summary>
        /// <param name="determineTargetConnectionsFunction">A function to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public ItemMovedNotification(Func<IEnumerable<IConnection>> determineTargetConnectionsFunction, ItemMovedNotificationArguments arguments)
        {
            determineTargetConnectionsFunction.ThrowIfNull(nameof(determineTargetConnectionsFunction));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = determineTargetConnectionsFunction;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public ItemMovedNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
        {
            var player = Game.Instance.GetCreatureWithId(this.Arguments.PlayerId);

            if (player.CanSee(this.Arguments.FromLocation) && this.Arguments.FromStackpos < 10)
            {
                this.Packets.Add(new RemoveAtStackposPacket(this.Arguments.FromLocation, this.Arguments.FromStackpos));
            }

            if (player.CanSee(this.Arguments.ToLocation))
            {
                this.Packets.Add(new AddItemPacket(this.Arguments.ToLocation, this.Arguments.Item));
            }
        }
    }
}