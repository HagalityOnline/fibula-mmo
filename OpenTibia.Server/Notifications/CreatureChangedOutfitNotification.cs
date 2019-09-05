// <copyright file="CreatureChangedOutfitNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;

    internal class CreatureChangedOutfitNotification : ProximityNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureChangedOutfitNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureChangedOutfitNotification(CreatureChangedOutfitNotificationArguments arguments)
            : base(arguments?.Creature.Location ?? default)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureChangedOutfitNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public override void Prepare()
        {
            this.Packets.Add(new CreatureChangedOutfitPacket(this.Arguments.Creature));
        }
    }
}