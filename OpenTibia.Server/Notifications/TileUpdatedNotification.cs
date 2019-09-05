// <copyright file="TileUpdatedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;

    internal class TileUpdatedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatedNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public TileUpdatedNotification(TileUpdatedNotificationArguments arguments)
            : base(playerId)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public TileUpdatedNotificationArguments Arguments { get; }

        public override void Prepare()
        {
            this.Packets.Add(new UpdateTilePacket(this.Arguments.Location, this.Arguments.Description));
        }
    }
}