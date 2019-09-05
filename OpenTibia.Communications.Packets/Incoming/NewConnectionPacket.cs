// <copyright file="NewConnectionPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class NewConnectionPacket : IIncomingPacket, INewConnectionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewConnectionPacket"/> class.
        /// </summary>
        /// <param name="operatingSystem"></param>
        /// <param name="version"></param>
        public NewConnectionPacket(ushort operatingSystem, ushort version)
        {
            this.Os = operatingSystem;
            this.Version = version;
        }

        public ushort Os { get; }

        public ushort Version { get; }
    }
}
