// -----------------------------------------------------------------
// <copyright file="PlayerMetadata.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Models
{
    using OpenTibia.Server.Contracts.Abstractions;

    public class PlayerMetadata : ICreatureMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerMetadata"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="maxHitpoints"></param>
        /// <param name="maxManapoints"></param>
        /// <param name="hitpoints"></param>
        /// <param name="manapoints"></param>
        /// <param name="corpse"></param>
        public PlayerMetadata(
            string name,
            ushort maxHitpoints,
            ushort maxManapoints,
            ushort hitpoints = 0,
            ushort manapoints = 0,
            ushort corpse = 0)
        {
            this.Name = name;
            this.MaxHitpoints = maxHitpoints;
            this.MaxManapoints = maxManapoints;
            this.Hitpoints = hitpoints > 0 ? hitpoints : maxHitpoints;
            this.Manapoints = manapoints > 0 ? manapoints : maxManapoints;
            this.Corpse = corpse;
        }

        public string Article { get; }

        public string Name { get; }

        public ushort Hitpoints { get; }

        public ushort MaxHitpoints { get; }

        public ushort Manapoints { get; }

        public ushort MaxManapoints { get; }

        public ushort Corpse { get; }
    }
}
