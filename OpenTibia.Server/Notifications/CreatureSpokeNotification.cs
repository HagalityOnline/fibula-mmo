// <copyright file="CreatureSpokeNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;

    internal class CreatureSpokeNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpokeNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureSpokeNotification(CreatureSpokeNotificationArguments arguments)
            : base(audience, playerId)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureSpokeNotificationArguments Arguments { get; }

        public override void Prepare()
        {
            this.Packets.Add(new CreatureSpeechPacket(this.Arguments.Creature.Name, this.Arguments.SpeechType, this.Arguments.Message, this.Arguments.Creature.Location, this.Arguments.Channel, (uint)DateTimeOffset.UtcNow.Ticks));
        }
    }
}