// <copyright file="WorldLightChangedNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    internal class WorldLightChangedNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldLightChangedNotificationArguments"/> class.
        /// </summary>
        /// <param name="lightLevel"></param>
        /// <param name="lightColor"></param>
        public WorldLightChangedNotificationArguments(byte lightLevel, byte lightColor = (byte)LightColors.White)
        {
            this.LightLevel = lightLevel;
            this.LightColor = lightColor;
        }

        public byte LightLevel { get; }

        public byte LightColor { get; }
    }
}