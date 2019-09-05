// <copyright file="ItemMovedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    internal class ItemMovedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMovedNotificationArguments"/> class.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fromLocation"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="toLocation"></param>
        /// <param name="toStackPos"></param>
        /// <param name="wasTeleport"></param>
        public ItemMovedNotificationArguments(IItem item, Location fromLocation, byte fromStackPos, Location toLocation, byte toStackPos, bool wasTeleport)
        {
            item.ThrowIfNull(nameof(item));

            this.Item = item;
            this.FromLocation = fromLocation;
            this.FromStackpos = fromStackPos;
            this.ToLocation = toLocation;
            this.ToStackpos = toStackPos;
            this.WasTeleport = wasTeleport;
        }

        public bool WasTeleport { get; }

        public byte FromStackpos { get; }

        public byte ToStackpos { get; }

        public Location FromLocation { get; }

        public Location ToLocation { get; }

        public IItem Item { get; }
    }
}