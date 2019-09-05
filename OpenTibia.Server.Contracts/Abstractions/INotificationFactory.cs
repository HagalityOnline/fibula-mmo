// -----------------------------------------------------------------
// <copyright file="INotificationFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Abstractions
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Interface for notification factories.
    /// </summary>
    public interface INotificationFactory
    {
        /// <summary>
        /// Creates a new notification based on the type and arguments supplied.
        /// </summary>
        /// <param name="type">The type of notification to create.</param>
        /// <param name="notificationArguments">The arguments for such notification.</param>
        /// <returns>The new notification instance.</returns>
        INotification Create(NotificationType type, INotificationArguments notificationArguments);
    }
}