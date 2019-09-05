// <copyright file="PlayerModel.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("players", Schema = "opentibia_classic")]
    public class PlayerModel : ICipPlayer
    {
        [Key]
        public short Player_Id { get; }

        public string Charname { get; }

        public int Account_Id { get; }

        public int Account_Nr { get; }

        public int Creation { get; }

        public int Lastlogin { get; }

        public byte Gender { get; }

        public byte Online { get; }

        public string Vocation { get; }

        public byte Hideprofile { get; }

        public int Playerdelete { get; }

        public short Level { get; }

        public string Residence { get; }

        public string Oldname { get; }

        public string Comment { get; }

        public string CharIp { get; }
    }
}
