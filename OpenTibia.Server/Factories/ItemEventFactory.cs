// <copyright file="ItemEventFactory.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Factories
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;
    using OpenTibia.Server.Events;

    using static OpenTibia.Server.Parsing.Grammar.EventGrammar;

    public class ItemEventFactory : IItemEventFactory
    {
        public IItemEvent Create(MoveUseEvent moveUseEvent)
        {
            moveUseEvent.ThrowIfNull(nameof(moveUseEvent));
            moveUseEvent.Rule.ThrowIfNull(nameof(moveUseEvent.Rule));

            if (!Enum.TryParse(moveUseEvent.Type, out ItemEventType eventType))
            {
                throw new ArgumentException($"Invalid rule '{moveUseEvent.Type}' supplied.");
            }

            switch (eventType)
            {
                case ItemEventType.Collision:
                    return new CollisionItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.Use:
                    return new UseItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.MultiUse:
                    return new MultiUseItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.Separation:
                    return new SeparationItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
                case ItemEventType.Movement:
                    return new MovementItemEvent(moveUseEvent.Rule.ConditionSet, moveUseEvent.Rule.ActionSet);
            }

            throw new InvalidCastException($"Unsuported type of event on EventFactory {moveUseEvent.Type}");
        }
    }
}
