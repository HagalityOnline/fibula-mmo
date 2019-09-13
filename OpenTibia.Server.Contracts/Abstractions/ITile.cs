// <copyright file="ITile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Structs;

    /// <summary>
    /// Interface for a map tile.
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// Gets the location of the tile.
        /// </summary>
        Location Location { get; }

        /// <summary>
        /// Gets the single ground item that a tile have.
        /// </summary>
        IItem Ground { get; }

        IReadOnlyCollection<IItem> TopItems1 { get; }

        IReadOnlyCollection<IItem> TopItems2 { get; }

        IReadOnlyCollection<IItem> DownItems { get; }

        IReadOnlyCollection<Guid> CreatureIds { get; }

        byte Flags { get; }

        bool IsHouse { get; }

        bool BlocksThrow { get; }

        bool BlocksPass { get; }

        bool BlocksLay { get; }

        bool HasCollisionEvents { get; }

        bool HasSeparationEvents { get; }

        IEnumerable<IItem> ItemsWithCollision { get; }

        IEnumerable<IItem> ItemsWithSeparation { get; }

        void AddThing(ref IThing thing, byte count = 1);

        void RemoveThing(ref IThing thing, byte count = 1);

        IThing GetThingAtStackPosition(byte stackPosition);

        byte GetStackPosition(IThing thing);

        void SetFlag(TileFlag flag);

        bool HasThing(IThing thing, byte count = 1);

        IItem BruteFindItemWithId(ushort typeId);

        IItem BruteRemoveItemWithId(ushort id);

        bool CanBeWalked(byte avoidDamageType = 0);

        /// <summary>
        /// Gets the tile description bytes as seen by a player.
        /// </summary>
        /// <param name="asPlayer">The player to which the tile is being descripted to.</param>
        /// <returns>The tile description bytes.</returns>
        IEnumerable<byte> GetDescription(IPlayer asPlayer);
    }
}
