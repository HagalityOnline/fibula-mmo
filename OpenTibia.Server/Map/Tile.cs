// <copyright file="Tile.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Communications.Contracts.Enumerations;
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Structs;
    using OpenTibia.Server.Items;
    using OpenTibia.Server.Parsing;

    /// <summary>
    /// Class that represents a tile in the map.
    /// </summary>
    public class Tile : ITile
    {
        /// <summary>
        /// The maximum number of things to describe.
        /// </summary>
        private const int MaximumNumberOfThingsToDescribe = 9;

        /// <summary>
        /// Stores the ids of the creatures in the tile.
        /// </summary>
        private readonly Stack<Guid> creatureIdsOnTile;

        /// <summary>
        /// Stores the 'top' items on the tile.
        /// </summary>
        private readonly Stack<IItem> topItems1OnTile;

        /// <summary>
        /// Stores the 'top 2' items on the tile.
        /// </summary>
        private readonly Stack<IItem> topItems2OnTile;

        /// <summary>
        /// Stores the down items on the tile.
        /// </summary>
        private readonly Stack<IItem> downItemsOnTile;

        /// <summary>
        /// Stores the content cache for this tile.
        /// </summary>
        private readonly IDictionary<string, object> contentCache;

        /// <summary>
        /// A lock to semaphore the content cache generation.
        /// </summary>
        private readonly object contentCacheLock;

        /// <summary>
        /// Stores the last time that this tile's content was edited.
        /// </summary>
        private DateTimeOffset contentLastEditionTime;

        /// <summary>
        /// Stores the last time that this tile's content cache was generated.
        /// </summary>
        private DateTimeOffset contentCacheGenerationTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="z">The Z coordinate of the tile.</param>
        public Tile(ICreatureFinder creatureFinder, ushort x, ushort y, sbyte z)
            : this(creatureFinder, new Location { X = x, Y = y, Z = z })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        /// <param name="creatureFinder">A reference to the creature finder.</param>
        /// <param name="location">The location of the tile.</param>
        public Tile(ICreatureFinder creatureFinder, Location location)
        {
            creatureFinder.ThrowIfNull(nameof(creatureFinder));

            this.CreatureFinder = creatureFinder;
            this.Location = location;

            this.creatureIdsOnTile = new Stack<Guid>();
            this.topItems1OnTile = new Stack<IItem>();
            this.topItems2OnTile = new Stack<IItem>();
            this.downItemsOnTile = new Stack<IItem>();

            this.contentCache = new Dictionary<string, object>();
            this.contentCacheLock = new object();
        }

        /// <summary>
        /// Gets the creature finder to use.
        /// </summary>
        public ICreatureFinder CreatureFinder { get; }

        /// <summary>
        /// Gets the tile's location.
        /// </summary>
        public Location Location { get; }

        /// <summary>
        /// Gets the tile's flags.
        /// </summary>
        public byte Flags { get; private set; }

        /// <summary>
        /// Gets the tile's ground.
        /// </summary>
        public IItem Ground { get; private set; }

        /// <summary>
        /// Gets the tile's creature ids.
        /// </summary>
        public IEnumerable<Guid> CreatureIds => this.creatureIdsOnTile;

        /// <summary>
        /// Gets the tile's 'top' items.
        /// </summary>
        public IEnumerable<IItem> TopItems1 => this.topItems1OnTile;

        /// <summary>
        /// Gets the tile's 'top 2' items.
        /// </summary>
        public IEnumerable<IItem> TopItems2 => this.topItems2OnTile;

        /// <summary>
        /// Gets the tile's down items.
        /// </summary>
        public IEnumerable<IItem> DownItems => this.downItemsOnTile;

        /// <summary>
        /// Gets a value indicating whether this tile has events that are triggered via collision evaluation.
        /// </summary>
        public bool HasCollisionEvents
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (bool)this.contentCache[nameof(this.HasCollisionEvents)];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tile has events that are triggered via separation events.
        /// </summary>
        public bool HasSeparationEvents
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (bool)this.contentCache[nameof(this.HasSeparationEvents)];
            }
        }

        /// <summary>
        /// Gets a collection of items in the tile that have collition events registered.
        /// </summary>
        public IEnumerable<IItem> ItemsWithCollision
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (IEnumerable<IItem>)this.contentCache[nameof(this.ItemsWithCollision)];
            }
        }

        /// <summary>
        /// Gets a collection of items in the tile that have separation events registered.
        /// </summary>
        public IEnumerable<IItem> ItemsWithSeparation
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (IEnumerable<IItem>)this.contentCache[nameof(this.ItemsWithSeparation)];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tile is part of a house.
        /// TODO: implement hosue system.
        /// </summary>
        public bool IsHouse => false;

        /// <summary>
        /// Gets a value indicating whether this tile blocks a throw.
        /// </summary>
        public bool BlocksThrow
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (bool)this.contentCache[nameof(this.BlocksThrow)];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tile blocks walking it.
        /// </summary>
        public bool BlocksPass
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (bool)this.contentCache[nameof(this.BlocksPass)];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this tile blocks laying items on top of it.
        /// </summary>
        public bool BlocksLay
        {
            get
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    // the cache content is no longer current, so we need to regenerate the cached content.
                    this.RegenerateContentCache();
                }

                return (bool)this.contentCache[nameof(this.BlocksLay)];
            }
        }

        /// <summary>
        /// Checks if the tile can be walked, optionally checking for an avoidance of damage type.
        /// </summary>
        /// <param name="avoidDamageType">Optional. A damage type to check for and avoid in the determination. Defaults to none.</param>
        /// <returns>True if the tile can be walked avoiding the damage type, false otherwise.</returns>
        public bool CanBeWalked(byte avoidDamageType = 0)
        {
            return !this.CreatureIds.Any()
                && this.Ground != null
                && !this.Ground.IsPathBlocking(avoidDamageType)
                && !this.TopItems1.Any(i => i.IsPathBlocking(avoidDamageType))
                && !this.TopItems2.Any(i => i.IsPathBlocking(avoidDamageType))
                && !this.DownItems.Any(i => i.IsPathBlocking(avoidDamageType));
        }

        /// <summary>
        /// Checks if the tile has a thing next in line.
        /// </summary>
        /// <param name="thing">The thing to check for.</param>
        /// <param name="count">A count of the thing to check for.</param>
        /// <returns>True if the thing and count is in the tile, false otherwise.</returns>
        public bool HasThing(IThing thing, byte count = 1)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.", nameof(count));
            }

            var creaturesCheck = thing is Creature creature && this.creatureIdsOnTile.Contains(creature.Id);

            var top1Check = thing is Item && this.topItems1OnTile.Count > 0 && this.topItems1OnTile.Peek() == thing && thing.Count >= count;
            var top2Check = thing is Item && this.topItems2OnTile.Count > 0 && this.topItems2OnTile.Peek() == thing && thing.Count >= count;
            var downCheck = thing is Item && this.downItemsOnTile.Count > 0 && this.downItemsOnTile.Peek() == thing && thing.Count >= count;

            return creaturesCheck || top1Check || top2Check || downCheck;
        }

        public void AddThing(ref IThing thing, byte count)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (thing is Creature creature)
            {
                this.creatureIdsOnTile.Push(creature.Id);
                creature.Tile = this;
                creature.Added();
            }
            else if (thing is Item item)
            {
                if (item.IsGround)
                {
                    this.Ground = item;
                    item.Added();
                }
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Push(item);
                    item.Added();
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Push(item);
                    item.Added();
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        var currentItem = this.downItemsOnTile.Count > 0 ? this.downItemsOnTile.Peek() as Item : null;

                        if (currentItem != null && currentItem.Type == item.Type && currentItem.Amount < 100)
                        {
                            // add these up.
                            var remaining = currentItem.Amount + count;

                            var newCount = (byte)Math.Min(remaining, 100);

                            currentItem.Amount = newCount;

                            remaining -= newCount;

                            if (remaining > 0)
                            {
                                IThing newThing = ItemFactory.Create(item.Type.TypeId);
                                this.AddThing(ref newThing, (byte)remaining);
                                thing = newThing;
                            }
                        }
                        else
                        {
                            item.Amount = count;
                            this.downItemsOnTile.Push(item);
                            item.Added();
                        }
                    }
                    else
                    {
                        this.downItemsOnTile.Push(item);
                        item.Added();
                    }
                }

                item.Tile = this;
            }
            else
            {
                throw new InvalidCastException("Thing did not cast to either a Creature or Item.");
            }

            this.contentLastEditionTime = DateTimeOffset.Now;
        }

        public void RemoveThing(ref IThing thing, byte count)
        {
            if (count == 0)
            {
                throw new ArgumentException("Invalid count zero.");
            }

            if (thing is Creature creature)
            {
                this.RemoveCreature(creature);
                creature.Tile = null;
                creature.Removed();
            }
            else if (thing is Item item)
            {
                var removeItem = true;

                if (item.IsGround)
                {
                    this.Ground = null;
                    item.Removed();
                    removeItem = false;
                }
                else if (item.IsTop1)
                {
                    this.topItems1OnTile.Pop();
                    item.Removed();
                    removeItem = false;
                }
                else if (item.IsTop2)
                {
                    this.topItems2OnTile.Pop();
                    item.Removed();
                    removeItem = false;
                }
                else
                {
                    if (item.IsCumulative)
                    {
                        if (item.Amount < count) // throwing because this should have been checked before.
                        {
                            throw new ArgumentException("Remove count is greater than available.");
                        }

                        if (item.Amount > count)
                        {
                            // create a new item (it got split...)
                            var newItem = ItemFactory.Create(item.Type.TypeId);
                            newItem.SetAmount(count);
                            item.Amount -= count;

                            thing = newItem;
                            removeItem = false;
                        }
                    }
                }

                if (removeItem)
                {
                    this.downItemsOnTile.Pop();
                    item.Removed();
                    item.Tile = null;
                }
            }
            else
            {
                throw new InvalidCastException("Thing did not cast to either a Creature or Item.");
            }

            this.contentLastEditionTime = DateTimeOffset.Now;
        }

        public void AddParsedContent(object contentObj)
        {
            if (!(contentObj is IEnumerable<CipElement> content))
            {
                return;
            }

            var downItemStackToReverse = new Stack<IItem>();
            var top1ItemStackToReverse = new Stack<IItem>();
            var top2ItemStackToReverse = new Stack<IItem>();

            foreach (var element in content)
            {
                if (element.Data < 0)
                {
                    // this is a flag an is unexpected.
                    // TODO: proper logging.
                    if (!ServerConfiguration.SupressInvalidItemWarnings)
                    {
                        Console.WriteLine($"Tile.AddContent: Unexpected flag {element.Attributes?.First()?.Name}, igoring.");
                    }

                    continue;
                }

                try
                {
                    var item = ItemFactory.Create((ushort)element.Data);

                    if (item == null)
                    {
                        if (!ServerConfiguration.SupressInvalidItemWarnings)
                        {
                            Console.WriteLine($"Tile.AddContent: Item with id {element.Data} not found in the catalog, skipping.");
                        }

                        continue;
                    }

                    item.AddAttributes(element.Attributes);

                    if (item.IsGround)
                    {
                        this.Ground = item;
                    }
                    else if (item.IsTop1)
                    {
                        top1ItemStackToReverse.Push(item);
                    }
                    else if (item.IsTop2)
                    {
                        top2ItemStackToReverse.Push(item);
                    }
                    else
                    {
                        downItemStackToReverse.Push(item);
                    }

                    item.Tile = this;
                }
                catch (ArgumentException)
                {
                    // TODO: proper logging.
                    if (!ServerConfiguration.SupressInvalidItemWarnings)
                    {
                        Console.WriteLine($"Tile.AddContent: Invalid item {element.Data} at {this.Location}, skipping.");
                    }
                }
            }

            // Reverse and add the stacks.
            while (top1ItemStackToReverse.Count > 0)
            {
                this.AddTopItem1(top1ItemStackToReverse.Pop());
            }

            while (top2ItemStackToReverse.Count > 0)
            {
                this.AddTopItem2(top2ItemStackToReverse.Pop());
            }

            while (downItemStackToReverse.Count > 0)
            {
                this.AddDownItem(downItemStackToReverse.Pop());
            }
        }

        public IItem BruteFindItemWithId(ushort id)
        {
            if (this.Ground != null && this.Ground.ThingId == id)
            {
                return this.Ground;
            }

            foreach (var item in this.topItems1OnTile.Union(this.topItems2OnTile).Union(this.downItemsOnTile))
            {
                if (item.ThingId == id)
                {
                    return item;
                }
            }

            return null;
        }

        public IItem BruteRemoveItemWithId(ushort id)
        {
            if (this.Ground != null && this.Ground.ThingId == id)
            {
                var ground = this.Ground;

                this.Ground = null;

                return ground;
            }

            var downItemStackToReverse = new Stack<IItem>();
            var top1ItemStackToReverse = new Stack<IItem>();
            var top2ItemStackToReverse = new Stack<IItem>();

            IItem itemFound = null;

            while (itemFound == null && this.topItems1OnTile.Count > 0)
            {
                var item = this.topItems1OnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    continue;
                }

                top1ItemStackToReverse.Push(item);
            }

            while (itemFound == null && this.topItems2OnTile.Count > 0)
            {
                var item = this.topItems2OnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    break;
                }

                top2ItemStackToReverse.Push(item);
            }

            while (itemFound == null && this.downItemsOnTile.Count > 0)
            {
                var item = this.downItemsOnTile.Pop();

                if (item.ThingId == id)
                {
                    itemFound = item;
                    break;
                }

                downItemStackToReverse.Push(item);
            }

            // Reverse and add the stacks back
            while (top1ItemStackToReverse.Count > 0)
            {
                this.AddTopItem1(top1ItemStackToReverse.Pop());
            }

            while (top2ItemStackToReverse.Count > 0)
            {
                this.AddTopItem2(top2ItemStackToReverse.Pop());
            }

            while (downItemStackToReverse.Count > 0)
            {
                this.AddDownItem(downItemStackToReverse.Pop());
            }

            return itemFound;
        }

        public IThing GetThingAtStackPosition(byte stackPosition)
        {
            if (stackPosition == 0 && this.Ground != null)
            {
                return this.Ground;
            }

            var currentPos = this.Ground == null ? -1 : 0;

            if (stackPosition > currentPos + this.topItems1OnTile.Count)
            {
                currentPos += this.topItems1OnTile.Count;
            }
            else
            {
                foreach (var item in this.TopItems1)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + this.topItems2OnTile.Count)
            {
                currentPos += this.topItems2OnTile.Count;
            }
            else
            {
                foreach (var item in this.TopItems2)
                {
                    if (++currentPos == stackPosition)
                    {
                        return item;
                    }
                }
            }

            if (stackPosition > currentPos + this.creatureIdsOnTile.Count)
            {
                currentPos += this.creatureIdsOnTile.Count;
            }
            else
            {
                foreach (var creatureId in this.CreatureIds)
                {
                    if (++currentPos == stackPosition)
                    {
                        return this.CreatureFinder.FindCreatureById(creatureId);
                    }
                }
            }

            return stackPosition <= currentPos + this.downItemsOnTile.Count ? this.DownItems.FirstOrDefault(item => ++currentPos == stackPosition) : null;
        }

        public byte GetStackPosition(IThing thing)
        {
            thing.ThrowIfNull(nameof(thing));

            if (this.Ground != null && thing == this.Ground)
            {
                return 0;
            }

            var n = 0;

            foreach (var item in this.TopItems1)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var item in this.TopItems2)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            foreach (var creatureId in this.CreatureIds)
            {
                ++n;

                if (thing is ICreature creature && creature.Id == creatureId)
                {
                    return (byte)n;
                }
            }

            foreach (var item in this.DownItems)
            {
                ++n;
                if (thing == item)
                {
                    return (byte)n;
                }
            }

            // return byte.MaxValue; // TODO: throw?
            throw new Exception("Thing not found in tile.");
        }

        /// <summary>
        /// Sets a flag on this tile.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        public void SetFlag(TileFlag flag)
        {
            this.Flags |= (byte)flag;
        }

        /// <summary>
        /// Gets the tile description bytes as seen by a player.
        /// </summary>
        /// <param name="asPlayer">The player to which the tile is being descripted to.</param>
        /// <returns>The tile description bytes.</returns>
        public IEnumerable<byte> GetDescription(IPlayer asPlayer)
        {
            var count = 0;

            var tempBytes = new List<byte>();

            this.GetFirstPartialDescription(ref tempBytes, ref count);
            this.GetCreaturesDescription(asPlayer, ref tempBytes, ref count);
            this.GetSecondPartialDescription(ref tempBytes, ref count);

            return tempBytes;
        }

        private void RegenerateContentCache()
        {
            lock (this.contentCacheLock)
            {
                if (this.contentLastEditionTime < this.contentCacheGenerationTime)
                {
                    return;
                }

                // collision events
                this.contentCache[nameof(this.HasCollisionEvents)] =
                    (this.Ground != null && this.Ground.HasCollision) ||
                    this.TopItems1.Any(i => i.HasCollision) ||
                    this.TopItems2.Any(i => i.HasCollision) ||
                    this.DownItems.Any(i => i.HasCollision);

                var items = new List<IItem>();

                if (this.Ground.HasCollision)
                {
                    items.Add(this.Ground);
                }

                items.AddRange(this.TopItems1.Where(i => i.HasCollision));
                items.AddRange(this.TopItems2.Where(i => i.HasCollision));
                items.AddRange(this.DownItems.Where(i => i.HasCollision));

                this.contentCache[nameof(this.ItemsWithCollision)] = items;

                // separation events
                this.contentCache[nameof(this.HasSeparationEvents)] =
                    (this.Ground != null && this.Ground.HasSeparation) ||
                    this.TopItems1.Any(i => i.HasSeparation) ||
                    this.TopItems2.Any(i => i.HasSeparation) ||
                    this.DownItems.Any(i => i.HasSeparation);

                items = new List<IItem>();

                if (this.Ground.HasSeparation)
                {
                    items.Add(this.Ground);
                }

                items.AddRange(this.TopItems1.Where(i => i.HasSeparation));
                items.AddRange(this.TopItems2.Where(i => i.HasSeparation));
                items.AddRange(this.DownItems.Where(i => i.HasSeparation));

                this.contentCache[nameof(this.ItemsWithSeparation)] = items;

                this.contentCache[nameof(this.BlocksThrow)] =
                    (this.Ground != null && this.Ground.BlocksThrow) ||
                    this.TopItems1.Any(i => i.BlocksThrow) ||
                    this.TopItems2.Any(i => i.BlocksThrow) ||
                    this.DownItems.Any(i => i.BlocksThrow);

                this.contentCache[nameof(this.BlocksPass)] =
                    (this.Ground != null && this.Ground.BlocksPass) ||
                    this.CreatureIds.Any() ||
                    this.TopItems1.Any(i => i.BlocksPass) ||
                    this.TopItems2.Any(i => i.BlocksPass) ||
                    this.DownItems.Any(i => i.BlocksPass);

                this.contentCache[nameof(this.BlocksLay)] =
                    (this.Ground != null && this.Ground.BlocksLay) ||
                    this.TopItems1.Any(i => i.BlocksLay) ||
                    this.TopItems2.Any(i => i.BlocksLay) ||
                    this.DownItems.Any(i => i.BlocksLay);

                // first partial description

                // second partial description

                this.contentCacheGenerationTime = DateTimeOffset.Now;
            }
        }

        private void GetCreaturesDescription(IPlayer asPlayer, ref List<byte> tempBytes, ref int count)
        {
            foreach (var creatureId in this.CreatureIds)
            {
                if (count == MaximumNumberOfThingsToDescribe)
                {
                    break;
                }

                var creature = this.CreatureFinder.FindCreatureById(creatureId);

                if (creature == null)
                {
                    continue;
                }

                if (asPlayer.KnowsCreatureWithId(creatureId))
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)OutgoingGamePacketType.AddKnownCreature));
                    tempBytes.AddRange(BitConverter.GetBytes((uint)creatureId.GetHashCode()));
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)OutgoingGamePacketType.AddUnknownCreature));
                    tempBytes.AddRange(BitConverter.GetBytes(asPlayer.ChooseToRemoveFromKnownSet()));
                    tempBytes.AddRange(BitConverter.GetBytes((uint)creatureId.GetHashCode()));

                    // TODO: is this the best spot for this ?
                    asPlayer.AddKnownCreature((uint)creatureId.GetHashCode());

                    var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                    tempBytes.AddRange(creatureNameBytes);
                }

                tempBytes.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
                tempBytes.Add((byte)creature.ClientSafeDirection);

                if (asPlayer.CanSee(creature))
                {
                    // Add creature outfit
                    tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.Id));

                    if (creature.Outfit.Id > 0)
                    {
                        tempBytes.Add(creature.Outfit.Head);
                        tempBytes.Add(creature.Outfit.Body);
                        tempBytes.Add(creature.Outfit.Legs);
                        tempBytes.Add(creature.Outfit.Feet);
                    }
                    else
                    {
                        tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.LikeType));
                    }
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                }

                tempBytes.Add(creature.LightBrightness);
                tempBytes.Add(creature.LightColor);

                tempBytes.AddRange(BitConverter.GetBytes(creature.Speed));

                tempBytes.Add(creature.Skull);
                tempBytes.Add(creature.Shield);
            }
        }

        /// <summary>
        /// Gets the tile's first partial description, which comes from the ground, top and top2 item stacks.
        /// </summary>
        /// <param name="tempBytes">A reference to the description bytes.</param>
        /// <param name="count">A reference to the current count of items described.</param>
        private void GetFirstPartialDescription(ref List<byte> tempBytes, ref int count)
        {
            if (this.Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(this.Ground.Type.ClientId));
                count++;
            }

            foreach (var item in this.TopItems1)
            {
                if (count == MaximumNumberOfThingsToDescribe)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            foreach (var item in this.TopItems2)
            {
                if (count == MaximumNumberOfThingsToDescribe)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }
        }

        /// <summary>
        /// Gets the tile's second partial description, which comes from the down item stack.
        /// </summary>
        /// <param name="tempBytes">A reference to the description bytes.</param>
        /// <param name="count">A reference to the current count of items described.</param>
        private void GetSecondPartialDescription(ref List<byte> tempBytes, ref int count)
        {
            foreach (var item in this.DownItems)
            {
                if (count == MaximumNumberOfThingsToDescribe)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }
        }

        /// <summary>
        /// Adds an item to the 'top' stack.
        /// </summary>
        /// <param name="i">The item to add.</param>
        private void AddTopItem1(IItem i)
        {
            lock (this.topItems1OnTile)
            {
                this.topItems1OnTile.Push(i);
                this.contentLastEditionTime = DateTimeOffset.Now;
            }
        }

        /// <summary>
        /// Adds an item to the 'top 2' stack.
        /// </summary>
        /// <param name="i">The item to add.</param>
        private void AddTopItem2(IItem i)
        {
            lock (this.topItems2OnTile)
            {
                this.topItems2OnTile.Push(i);
                this.contentLastEditionTime = DateTimeOffset.Now;
            }
        }

        /// <summary>
        /// Adds an item to the 'down' stack.
        /// </summary>
        /// <param name="i">The item to add.</param>
        private void AddDownItem(IItem i)
        {
            lock (this.downItemsOnTile)
            {
                this.downItemsOnTile.Push(i);
                this.contentLastEditionTime = DateTimeOffset.Now;
            }
        }

        /// <summary>
        /// Removes a creature from the tile.
        /// </summary>
        /// <param name="c">The creature to complete.</param>
        private void RemoveCreature(ICreature c)
        {
            var tempStack = new Stack<Guid>();

            ICreature removed = null;

            lock (this.creatureIdsOnTile)
            {
                while (removed == null && this.creatureIdsOnTile.Count > 0)
                {
                    var temp = this.creatureIdsOnTile.Pop();

                    if (c.Id == temp)
                    {
                        removed = c;
                    }
                    else
                    {
                        tempStack.Push(temp);
                    }
                }

                while (tempStack.Count > 0)
                {
                    this.creatureIdsOnTile.Push(tempStack.Pop());
                }
            }

            // Console.WriteLine($"Removed creature {c.Name} at {this.Location}");
        }

        // public FloorChangeDirection FloorChange
        // {
        //    get
        //    {
        //        if (Ground.HasFlag(ItemFlag.FloorchangeDown))
        //        {
        //            return FloorChangeDirection.Down;
        //        }
        //        else
        //        {
        //            foreach (IItem item in TopItems1)
        //            {
        //                if (item.HasFlag(ItemFlag.TopOrder3))
        //                {
        //                    switch (item.Type)
        //                    {
        //                        case 1126:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.East);
        //                        case 1128:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.West);
        //                        case 1130:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.South);
        //                        default:
        //                        case 1132:
        //                            return (FloorChangeDirection.Up | FloorChangeDirection.North);
        //                    }
        //                }
        //            }
        //        }

        // return FloorChangeDirection.None;
        //    }
        // }

        // public bool IsWalkable { get { return Ground != null && !HasFlag(ItemFlag.Blocking); } }

        // public bool HasFlag(ItemFlag flagVal)
        // {
        //    if (Ground != null)
        //    {
        //        if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //            return true;
        //    }

        // if (TopItems1.Count > 0)
        //    {
        //        foreach (IItem item in TopItems1)
        //        {
        //            if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //                return true;
        //        }
        //    }

        // if (TopItems2.Count > 0)
        //    {
        //        foreach (IItem item in TopItems2)
        //        {
        //            if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //                return true;
        //        }
        //    }

        // if (CreatureIds.Count > 0)
        //    {
        //        foreach (CreatureId creature in CreatureIds)
        //        {
        //            if (flagVal == ItemFlag.Blocking)
        //                return true;
        //        }
        //    }

        // if (DownItems.Count > 0)
        //    {
        //        foreach (IItem item in DownItems)
        //        {
        //            if (ItemReader.FindItem(Ground.Type).hasFlag(flagVal))
        //                return true;
        //        }
        //    }
        //    return false;
        // }
    }
}