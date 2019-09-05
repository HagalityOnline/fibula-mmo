// -----------------------------------------------------------------
// <copyright file="World.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Configuration
{
    using OpenTibia.Server.Contracts.Enumerations;

    public class World
    {
        /// <summary>
        /// Gets or sets the world type.
        /// </summary>
        public WorldType Type { get; set; }

        public string Name { get; set; }

        public string WebsiteUrl { get; set; }

        public string Geolocation { get; set; }

        public byte LocalResetHour { get; set; }

        public string MessageOfTheDay { get; set; }

        public ushort MaximumOnlineVeterans { get; set; }

        public ushort MaximumOnlineNewbies { get; set; }

        public ushort MaximumVeteranQueueSize { get; set; }

        public ushort MaximumNewbieQueueSize { get; set; }
    }
}
