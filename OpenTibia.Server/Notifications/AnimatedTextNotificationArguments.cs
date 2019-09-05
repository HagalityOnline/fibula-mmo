// <copyright file="AnimatedTextNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Internal class that represents arguments for an animated text notification.
    /// </summary>
    internal class AnimatedTextNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedTextNotificationArguments"/> class.
        /// </summary>
        /// <param name="location">The location of the animated text.</param>
        /// <param name="text">The text.</param>
        /// <param name="textColor">The color of the text.</param>
        public AnimatedTextNotificationArguments(Location location, string text, TextColor textColor = TextColor.White)
        {
            text.ThrowIfNullOrWhiteSpace(nameof(text));

            this.Location = location;
            this.Text = text;
            this.TextColor = textColor;
        }

        /// <summary>
        /// Gets the location of the animated text.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the color of the text.
        /// </summary>
        public TextColor TextColor { get; }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; }
    }
}