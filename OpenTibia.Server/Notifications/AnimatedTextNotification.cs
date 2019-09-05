// <copyright file="AnimatedTextNotification.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Packets.Outgoing;

    /// <summary>
    /// Class that represents a notification for animated text to players who are close.
    /// </summary>
    internal class AnimatedTextNotification : ProximityNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedTextNotification"/> class.
        /// </summary>
        /// <param name="arguments">The arguments for this notification.</param>
        public AnimatedTextNotification(AnimatedTextNotificationArguments arguments)
            : base(arguments?.Location ?? default)
        {
            arguments.ThrowIfNull(nameof(arguments));

            this.Arguments = arguments;
        }

        /// <summary>
        /// Gets this notification's arguments.
        /// </summary>
        public AnimatedTextNotificationArguments Arguments { get; }

        /// <summary>
        /// Finalizes the notification in preparation to it being sent.
        /// </summary>
        public override void Prepare()
        {
            this.Packets.Add(new AnimatedTextPacket(this.Arguments.Location, this.Arguments.TextColor, this.Arguments.Text));
        }
    }
}