// <copyright file="IHighscore.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IHighscore
    {
        int AccountId { get; }

        string Charname { get; }

        string Vocation { get; }

        int Level { get; }

        byte Exp { get; }

        byte Mlvl { get; }

        byte SkillShield { get; }

        byte SkillDist { get; }

        byte SkillAxe { get; }

        byte SkillSword { get; }

        byte SkillClub { get; }

        byte SkillFist { get; }

        byte SkillFish { get; }
    }
}
