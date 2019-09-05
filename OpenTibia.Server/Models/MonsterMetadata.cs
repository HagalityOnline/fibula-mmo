// -----------------------------------------------------------------
// <copyright file="MonsterMetadata.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Models
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Monsters;

    public class MonsterMetadata : ICreatureMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonsterMetadata"/> class.
        /// </summary>
        /// <param name="monsterType"></param>
        public MonsterMetadata(MonsterType monsterType)
        {
            monsterType.ThrowIfNull(nameof(monsterType));

            this.Type = monsterType;
        }

        public string Article => this.Type.Article;

        public string Name => this.Type.Name;

        public ushort Hitpoints => this.Type.MaxHitPoints;

        public ushort MaxHitpoints => this.Type.MaxHitPoints;

        public ushort Manapoints => this.Type.MaxManaPoints;

        public ushort MaxManapoints => this.Type.MaxManaPoints;

        public ushort Corpse => this.Type.Corpse;

        public MonsterType Type { get; }
    }
}
