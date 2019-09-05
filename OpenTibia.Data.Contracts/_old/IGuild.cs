// <copyright file="IGuild.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IGuild
    {
        short GuildId { get; }

        string GuildName { get; }

        int GuildOwner { get; }

        string Description { get; }

        int Ts { get; }

        byte Ranks { get; }

        string Rank1 { get; }

        string Rank2 { get; }

        string Rank3 { get; }

        string Rank4 { get; }

        string Rank5 { get; }

        string Rank6 { get; }

        string Rank7 { get; }

        string Rank8 { get; }

        string Rank9 { get; }

        string Rank10 { get; }

        string Logo { get; }
    }
}
