// <copyright file="IPlayerLoginInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    public interface IPlayerLoginInfo
    {
        ushort Os { get; }

        ushort Version { get; }

        uint[] XteaKey { get; }

        bool IsGm { get; }

        uint AccountNumber { get; }

        string CharacterName { get; }

        string Password { get; }
    }
}