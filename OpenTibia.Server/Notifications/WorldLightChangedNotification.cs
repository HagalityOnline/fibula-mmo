// <copyright file="WorldLightChangedNotification.cs" company="2Dudes">
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

    /// <summary>
    /// Class that represents a notification to all players that world light has changed.
    /// </summary>
    internal class WorldLightChangedNotification : AllPlayersNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldLightChangedNotification"/> class.
        /// </summary>
        /// <param name="targetConnectionsFunc">A reference to determine the target connections of this notification.</param>
        /// <param name="arguments">The arguments for this notification.</param>
        public WorldLightChangedNotification(Func<IEnumerable<IConnection>> targetConnectionsFunc, WorldLightChangedNotificationArguments arguments)
            : base(targetConnectionsFunc)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public WorldLightChangedNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public override void Prepare()
        {
            this.Packets.Add(new WorldLightPacket(this.Arguments.LightLevel, this.Arguments.LightColor));
        }
    }
}