// <copyright file="Guild.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("guilds", Schema = "opentibia_classic")]
    public class Guild : IGuild
    {
        [Key]
        public short GuildId { get; }

        public string GuildName { get; }

        public int GuildOwner { get; }

        public string Description { get; }

        public int Ts { get; }

        public byte Ranks { get; }

        public string Rank1 { get; }

        public string Rank2 { get; }

        public string Rank3 { get; }

        public string Rank4 { get; }

        public string Rank5 { get; }

        public string Rank6 { get; }

        public string Rank7 { get; }

        public string Rank8 { get; }

        public string Rank9 { get; }

        public string Rank10 { get; }

        public string Logo { get; }
    }
}
