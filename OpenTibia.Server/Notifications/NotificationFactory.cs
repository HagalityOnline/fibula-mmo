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
    using System.Linq;
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
        /// <param name="creatureFinder">A reference to the creature manager.</param>
        public NotificationFactory(
            IConnectionManager connectionManager,
            ICreatureFinder creatureFinder)
        {
            this.ConnectionManager = connectionManager;
            this.CreatureFinder = creatureFinder;
        }

        /// <summary>
        /// Gets the reference to the connection manager.
        /// </summary>
        public IConnectionManager ConnectionManager { get; }

        /// <summary>
        /// Gets the reference to the creature finder.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

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
                        return new GenericNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => genericNotificationArguments.PlayerIds.Contains(c.PlayerId)),
                            genericNotificationArguments);
                    }

                    break;
                case NotificationType.AnimatedText:

                    if (notificationArguments is AnimatedTextNotificationArguments animatedTextNotificationArguments)
                    {
                        return new AnimatedTextNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(animatedTextNotificationArguments.Location) ?? false),
                            animatedTextNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureAdded:

                    if (notificationArguments is CreatureAddedNotificationArguments creatureAddedNotificationArguments)
                    {
                        return new CreatureAddedNotification(
                            this.CreatureFinder,
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(creatureAddedNotificationArguments.Creature) ?? false),
                            creatureAddedNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureChangedOutfit:

                    if (notificationArguments is CreatureChangedOutfitNotificationArguments creatureChangedOutfitNotificationArguments)
                    {
                        return new CreatureChangedOutfitNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(creatureChangedOutfitNotificationArguments.Creature) ?? false),
                            creatureChangedOutfitNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureMoved:

                    if (notificationArguments is CreatureMovedNotificationArguments creatureMovedNotificationArguments)
                    {
                        return new CreatureMovedNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(creatureMovedNotificationArguments.Location) ?? false),
                            creatureMovedNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureRemoved:

                    if (notificationArguments is CreatureRemovedNotificationArguments creatureRemovedNotificationArguments)
                    {
                        return new CreatureRemovedNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(creatureRemovedNotificationArguments.Creature) ?? false),
                            creatureRemovedNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureSpoke:

                    if (notificationArguments is CreatureSpokeNotificationArguments creatureSpokeNotificationArguments)
                    {
                        return new CreatureSpokeNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(creatureSpokeNotificationArguments.Creature) ?? false),
                            creatureSpokeNotificationArguments);
                    }

                    break;
                case NotificationType.CreatureTurned:

                    if (notificationArguments is CreatureTurnedNotificationArguments creatureTurnedNotificationArguments)
                    {
                        return new CreatureTurnedNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(creatureTurnedNotificationArguments.Creature) ?? false),
                            creatureTurnedNotificationArguments);
                    }

                    break;
                case NotificationType.ItemMoved:

                    if (notificationArguments is ItemMovedNotificationArguments itemMovedNotificationArguments)
                    {
                        return new ItemMovedNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(itemMovedNotificationArguments.Location) ?? false),
                            itemMovedNotificationArguments);
                    }

                    break;
                case NotificationType.TileUpdated:

                    if (notificationArguments is TileUpdatedNotificationArguments tileUpdatedNotificationArguments)
                    {
                        return new TileUpdatedNotification(
                            () => this.ConnectionManager.GetAllActive().Where(c => this.CreatureFinder.FindCreatureById(c.PlayerId)?.CanSee(tileUpdatedNotificationArguments.Location) ?? false),
                            tileUpdatedNotificationArguments);
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
