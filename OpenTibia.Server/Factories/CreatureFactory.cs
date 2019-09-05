// -----------------------------------------------------------------
// <copyright file="CreatureFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Factories
{
    using System;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Models;
    using OpenTibia.Server.Monsters;

    public class CreatureFactory : ICreatureFactory
    {
        public ICreature Create(IGame gameInstance, CreatureType type, ICreatureMetadata creatureMetadata)
        {
            switch (type)
            {
                case CreatureType.NonPlayerCharacter:

                    //if (creatureMetadata is NonPlayerCharacterMetadata npcMetadata)
                    //{
                    //    return new NonPlayerCharacter(
                    //        npcMetadata.CreatureId,
                    //        npcMetadata.Name,
                    //        npcMetadata.MaxHitpoints,
                    //        npcMetadata.MaxManapoints,
                    //        npcMetadata.Corpse,
                    //        npcMetadata.Hitpoints,
                    //        npcMetadata.Manapoints);
                    //}

                    //throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(NonPlayerCharacterMetadata)} when {type} is used.");

                case CreatureType.Player:

                    if (creatureMetadata is PlayerMetadata playerMetadata)
                    {
                        return new Player(
                            gameInstance,
                            (uint)new Random(DateTimeOffset.Now.Millisecond).Next(), // TODO: actual random or player Id here?
                            playerMetadata.Name,
                            playerMetadata.MaxHitpoints,
                            playerMetadata.MaxManapoints,
                            playerMetadata.Corpse,
                            playerMetadata.Hitpoints,
                            playerMetadata.Manapoints);
                    }

                    throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(PlayerMetadata)} when {type} is used.");

                case CreatureType.Monster:

                    if (creatureMetadata is MonsterMetadata monsterMetadata)
                    {
                        return new Monster(gameInstance, monsterMetadata.Type);
                    }

                    throw new InvalidCastException($"{nameof(creatureMetadata)} must be castable to {nameof(MonsterMetadata)} when {type} is used.");
            }

            throw new NotSupportedException($"{nameof(CreatureFactory)} does not support creation of creatures with type {type}.");
        }
    }
}
