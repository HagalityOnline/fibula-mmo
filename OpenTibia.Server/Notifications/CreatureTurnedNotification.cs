// <copyright file="CreatureTurnedNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;
    using OpenTibia.Data.Contracts.Enumerations;

    internal class CreatureTurnedNotification : Notification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureTurnedNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public CreatureTurnedNotification(CreatureTurnedNotificationArguments arguments)
            : base(audience, playerId)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public CreatureTurnedNotificationArguments Arguments { get; }

        public override void Prepare()
        {
            if (this.Arguments.TurnedEffect != AnimatedEffect.None)
            {
                this.Packets.Add(new MagicEffectPacket(this.Arguments.Creature.Location, this.Arguments.TurnedEffect));
            }

            this.Packets.Add(new CreatureTurnedPacket(this.Arguments.Creature));
        }
    }
}