// -----------------------------------------------------------------
// <copyright file="NotificationFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Notifications
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Class that represents a factory for notifications.
    /// </summary>
    public class NotificationFactory : INotificationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationFactory"/> class.
        /// </summary>
        /// <param name="connectionManager">A reference to the connection manager.</param>
        public NotificationFactory(IConnectionManager connectionManager)
        {
            this.ConnectionManager = connectionManager;
        }

        /// <summary>
        /// Gets the reference to the connection manager.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Creates a new notification based on the type and arguments supplied.
        /// </summary>
        /// <param name="type">The type of notification to create.</param>
        /// <param name="notificationArguments">The arguments for such notification.</param>
        /// <returns>The new notification instance.</returns>
        public INotification Create(NotificationType type, INotificationArguments notificationArguments)
        {
            notificationArguments.ThrowIfNull(nameof(notificationArguments));

            switch (type)
            {
                case NotificationType.Generic:

                    if (notificationArguments is GenericNotificationArguments genericNotificationArguments)
                    {
                        return new GenericNotification(this.ConnectionManager.GetAllActive, genericNotificationArguments);
                    }

                    break;
                case NotificationType.AnimatedText:

                    if (notificationArguments is AnimatedTextNotificationArguments animatedTextNotificationArguments)
                    {
                        return new AnimatedTextNotification(animatedTextNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureAdded:

                    if (notificationArguments is CreatureAddedNotificationArguments creatureAddedNotificationArguments)
                    {
                        return new CreatureAddedNotification(creatureAddedNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureChangedOutfit:

                    if (notificationArguments is CreatureChangedOutfitNotificationArguments creatureChangedOutfitNotificationArguments)
                    {
                        return new CreatureChangedOutfitNotification(creatureChangedOutfitNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureMoved:

                    if (notificationArguments is CreatureMovedNotificationArguments creatureMovedNotificationArguments)
                    {
                        return new CreatureMovedNotification(creatureMovedNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureRemoved:

                    if (notificationArguments is CreatureRemovedNotificationArguments creatureRemovedNotificationArguments)
                    {
                        return new CreatureRemovedNotification(creatureRemovedNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureSpoke:

                    if (notificationArguments is CreatureSpokeNotificationArguments creatureSpokeNotificationArguments)
                    {
                        return new CreatureSpokeNotification(creatureSpokeNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureTurned:

                    if (notificationArguments is CreatureTurnedNotificationArguments creatureTurnedNotificationArguments)
                    {
                        return new CreatureTurnedNotification(creatureTurnedNotificationArguments);
                    }

                    break;
                case NotificationType.ItemMoved:

                    if (notificationArguments is ItemMovedNotificationArguments itemMovedNotificationArguments)
                    {
                        return new ItemMovedNotification(itemMovedNotificationArguments);
                    }

                    break;
                case NotificationType.TileUpdated:

                    if (notificationArguments is TileUpdatedNotificationArguments tileUpdatedNotificationArguments)
                    {
                        return new TileUpdatedNotification(tileUpdatedNotificationArguments);
                    }

                    break;
                case NotificationType.WorldLightChanged:

                    if (notificationArguments is WorldLightChangedNotificationArguments worldLightChangedNotificationArguments)
                    {
                        return new WorldLightChangedNotification(this.ConnectionManager.GetAllActive, worldLightChangedNotificationArguments);
                    }

                    break;
            }

            throw new NotSupportedException($"Unsupported notification type '{type}' or wrong arguments ({notificationArguments.GetType().Name}) supplied.");
        }
    }
}
