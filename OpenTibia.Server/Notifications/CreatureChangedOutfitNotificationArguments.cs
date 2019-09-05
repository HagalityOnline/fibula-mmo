// <copyright file="CreatureChangedOutfitNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class CreatureChangedOutfitNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureChangedOutfitNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature"></param>
        public CreatureChangedOutfitNotificationArguments(ICreature creature)
        {
            creature.ThrowIfNull(nameof(creature));

            this.Creature = creature;
        }

        public ICreature Creature { get; }
    }
}