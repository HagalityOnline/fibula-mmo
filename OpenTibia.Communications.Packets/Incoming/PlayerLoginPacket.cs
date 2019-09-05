// <copyright file="PlayerLoginPacket.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Incoming
{
    using OpenTibia.Communications.Contracts.Abstractions;
    using OpenTibia.Communications.Packets.Contracts.Abstractions;

    public class PlayerLoginPacket : IIncomingPacket, IPlayerLoginInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerLoginPacket"/> class.
        /// </summary>
        /// <param name="xteaKey"></param>
        /// <param name="operatingSystem"></param>
        /// <param name="version"></param>
        /// <param name="isGamemaster"></param>
        /// <param name="accountNumber"></param>
        /// <param name="characterName"></param>
        /// <param name="password"></param>
        public PlayerLoginPacket(uint[] xteaKey, ushort operatingSystem, ushort version, bool isGamemaster, uint accountNumber, string characterName, string password)
        {
            this.XteaKey = xteaKey;

            this.Os = operatingSystem;
            this.Version = version;

            this.IsGm = isGamemaster;

            this.AccountNumber = accountNumber;
            this.CharacterName = characterName;
            this.Password = password;
        }

        public ushort Os { get; }

        public ushort Version { get; }

        public uint[] XteaKey { get; }

        public bool IsGm { get; }

        public uint AccountNumber { get; }

        public string CharacterName { get; }

        public string Password { get; }
    }
}
