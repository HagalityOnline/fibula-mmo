// <copyright file="Highscore.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("highscores", Schema = "opentibia_classic")]
    public class Highscore : IHighscore
    {
        [Key]
        public int AccountId { get; }

        public string Charname { get; }

        public string Vocation { get; }

        public int Level { get; }

        public byte Exp { get; }

        public byte Mlvl { get; }

        public byte SkillShield { get; }

        public byte SkillDist { get; }

        public byte SkillAxe { get; }

        public byte SkillSword { get; }

        public byte SkillClub { get; }

        public byte SkillFist { get; }

        public byte SkillFish { get; }
    }
}
