// <copyright file="DefaultReadPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class DefaultReadPacket : IIncomingPacket, IDefaultInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultReadPacket"/> class.
        /// </summary>
        /// <param name="bytes"></param>
        public DefaultReadPacket(params byte[] bytes)
        {
            bytes.ThrowIfNull(nameof(bytes));

            this.Bytes = bytes;
        }

        public byte[] Bytes { get; }
    }
}
