// <copyright file="GameConfigurationOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Configuration
{
    public class GameConfigurationOptions
    {
        public World World { get; set; }

        public AddressBinding PublicAddressBinding { get; set; }

        public AddressBinding PrivateAddressBinding { get; set; }
    }
}
