// <copyright file="TileUpdatedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    internal class TileUpdatedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileUpdatedNotificationArguments"/> class.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="location"></param>
        /// <param name="description"></param>
        public TileUpdatedNotificationArguments(Location location, byte[] description)
        {
            this.Location = location;
            this.Description = description;
        }

        public Location Location { get; }

        public byte[] Description { get; }
    }
}