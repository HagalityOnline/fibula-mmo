﻿// -----------------------------------------------------------------
// <copyright file="GrabberHasEnoughCarryStrengthEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.MovementEvents.EventConditions
{
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Server;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a condition which evaluates if a grabber has enough carry strength in them to pick up a thing.
    /// </summary>
    internal class GrabberHasEnoughCarryStrengthEventCondition : IEventCondition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrabberHasEnoughCarryStrengthEventCondition"/> class.
        /// </summary>
        /// <param name="pickerId">The id of the creature picking the thing.</param>
        /// <param name="thingPicking">The thing being picked.</param>
        /// <param name="thingDropping">The thing being dropped.</param>
        public GrabberHasEnoughCarryStrengthEventCondition(uint pickerId, IThing thingPicking, IThing thingDropping = null)
        {
            this.PickerId = pickerId;
            this.ThingPicking = thingPicking;
            this.ThingDropping = thingDropping is IContainer ? null : thingDropping; // We're actually trying to put this item in, so no dropping is happening.
        }

        /// <summary>
        /// Gets the <see cref="IThing"/> that is being picked.
        /// </summary>
        public IThing ThingPicking { get; }

        /// <summary>
        /// Gets the <see cref="IThing"/> that is being dropped.
        /// </summary>
        public IThing ThingDropping { get; }

        /// <summary>
        /// Gets the picker id.
        /// </summary>
        public uint PickerId { get; }

        /// <inheritdoc/>
        public string ErrorMessage => "The object is too heavy.";

        /// <inheritdoc/>
        public bool Evaluate()
        {
            var itemBeingDropped = this.ThingDropping as IItem;
            var picker = this.PickerId == 0 ? null : Game.Instance.GetCreatureWithId(this.PickerId);

            if (!(this.ThingPicking is IItem itemBeingPicked) || picker == null)
            {
                return false;
            }

            return picker is IPlayer && (picker as IPlayer).AccessLevel > 0 ||
                picker.CarryStrength - itemBeingPicked.Weight + (itemBeingDropped?.Weight ?? 0) >= 0;
        }
    }
}