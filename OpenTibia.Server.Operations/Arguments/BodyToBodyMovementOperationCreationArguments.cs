﻿// -----------------------------------------------------------------
// <copyright file="BodyToBodyMovementOperationCreationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Operations.Arguments
{
    using OpenTibia.Common.Utilities;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    public class BodyToBodyMovementOperationCreationArguments : IOperationCreationArguments
    {
        public BodyToBodyMovementOperationCreationArguments(uint requestorId, IThing thingMoving, ICreature targetCreature, Slot fromSlot, Slot toSlot, byte amount = 1)
        {
            thingMoving.ThrowIfNull(nameof(thingMoving));
            targetCreature.ThrowIfNull(nameof(targetCreature));

            this.RequestorId = requestorId;
            this.ThingMoving = thingMoving;
            this.TargetCreature = targetCreature;
            this.FromSlot = fromSlot;
            this.ToSlot = toSlot;
            this.Amount = amount;
        }

        public uint RequestorId { get; }

        public IThing ThingMoving { get; }

        public ICreature TargetCreature { get; }

        public Slot FromSlot { get; }

        public Slot ToSlot { get; }

        public byte Amount { get; }
    }
}