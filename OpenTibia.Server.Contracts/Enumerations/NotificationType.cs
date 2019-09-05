// <copyright file="NotificationType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumerates de different notification types.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// The generic notification.
        /// </summary>
        Generic,

        /// <summary>
        /// Animated text shown in a particular location.
        /// </summary>
        AnimatedText,

        /// <summary>
        /// A creature has been added in sight.
        /// </summary>
        CreatureAdded,

        /// <summary>
        /// A creature has changed outfit.
        /// </summary>
        CreatureChangedOutfit,

        /// <summary>
        /// A creature moved.
        /// </summary>
        CreatureMoved,

        /// <summary>
        /// A creature was removed from sight.
        /// </summary>
        CreatureRemoved,

        /// <summary>
        /// A creature spoke.
        /// </summary>
        CreatureSpoke,

        /// <summary>
        /// A creature turned.
        /// </summary>
        CreatureTurned,

        /// <summary>
        /// An item was moved.
        /// </summary>
        ItemMoved,

        /// <summary>
        /// A tile was updated.
        /// </summary>
        TileUpdated,

        /// <summary>
        /// The world light changed.
        /// </summary>
        WorldLightChanged,
    }
}
