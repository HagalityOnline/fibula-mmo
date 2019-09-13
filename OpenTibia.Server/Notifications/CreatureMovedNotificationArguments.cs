// <copyright file="CreatureMovedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;

    internal class CreatureMovedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureMovedNotificationArguments"/> class.
        /// </summary>
        /// <param name="creatureId"></param>
        /// <param name="fromLocation"></param>
        /// <param name="fromStackPos"></param>
        /// <param name="toLocation"></param>
        /// <param name="toStackPos"></param>
        /// <param name="wasTeleport"></param>
        public CreatureMovedNotificationArguments(Guid creatureId, Location fromLocation, byte fromStackPos, Location toLocation, byte toStackPos, bool wasTeleport)
        {
            var locationDiff = fromLocation - toLocation;

            this.CreatureId = creatureId;
            this.OldLocation = fromLocation;
            this.OldStackPosition = fromStackPos;
            this.NewLocation = toLocation;
            this.NewStackPosition = toStackPos;
            this.WasTeleport = wasTeleport || locationDiff.MaxValueIn3D > 1;
        }

        public bool WasTeleport { get; }

        public byte OldStackPosition { get; }

        public byte NewStackPosition { get; }

        public Location OldLocation { get; }

        public Location NewLocation { get; }

        public Guid CreatureId { get; }
    }
}