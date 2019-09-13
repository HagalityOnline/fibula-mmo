// <copyright file="GenericNotification.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents a generic notification.
    /// </summary>
    internal class GenericNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericNotification"/> class.
        /// </summary>
        /// <param name="targetConnectionsFunc">A reference to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public GenericNotification(Func<IEnumerable<IConnection>> targetConnectionsFunc, GenericNotificationArguments arguments)
        {
            targetConnectionsFunc.ThrowIfNull(nameof(targetConnectionsFunc));
            arguments.ThrowIfNull(nameof(arguments));

            this.TargetConnectionsFunction = targetConnectionsFunc;
            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public GenericNotificationArguments Arguments { get; }

        /// <summary>
        /// Gets the function for determining target connections for this notification.
        /// </summary>
        protected override Func<IEnumerable<IConnection>> TargetConnectionsFunction { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        protected override void Prepare()
        {
            foreach (var packet in this.Arguments.OutgoingPackets)
            {
                this.Packets.Add(packet);
            }
        }
    }
}