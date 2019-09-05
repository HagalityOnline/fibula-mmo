// <copyright file="MovementItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Enumerations;

    internal class MovementItemEvent : BaseItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovementItemEvent"/> class.
        /// </summary>
        /// <param name="conditionSet"></param>
        /// <param name="actionSet"></param>
        public MovementItemEvent(IList<string> conditionSet, IList<string> actionSet)
            : base(conditionSet, actionSet)
        {
        }

        public override ItemEventType Type => ItemEventType.Movement;
    }
}