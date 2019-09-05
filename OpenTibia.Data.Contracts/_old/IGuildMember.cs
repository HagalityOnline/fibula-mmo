// <copyright file="IGuildMember.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IGuildMember
    {
        int EntryId { get; }

        int AccountId { get; }

        short GuildId { get; }

        string GuildTitle { get; }

        string PlayerTitle { get; }

        byte Invitation { get; }

        int Timestamp { get; }

        byte Rank { get; }
    }
}
