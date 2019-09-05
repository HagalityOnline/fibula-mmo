// <copyright file="GuildMember.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("guild_members", Schema = "opentibia_classic")]
    public class GuildMember : IGuildMember
    {
        [Key]
        public int EntryId { get; }

        public int AccountId { get; }

        public short GuildId { get; }

        public string GuildTitle { get; }

        public string PlayerTitle { get; }

        public byte Invitation { get; }

        public int Timestamp { get; }

        public byte Rank { get; }
    }
}
