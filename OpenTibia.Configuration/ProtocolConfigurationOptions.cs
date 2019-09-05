// <copyright file="ProtocolConfigurationOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Configuration
{
    public class ProtocolConfigurationOptions
    {
        public ProtocolVersion ServerVersion { get; set; }

        public ProtocolVersion ClientVersion { get; set; }

        public bool UsingCipsoftRsaKeys { get; set; }

        public string QueryManagerPassword { get; set; }
    }
}
