// <copyright file="CollisionItemEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenTibia.Server.Contracts.Enumerations;

    internal class CollisionItemEvent : BaseItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionItemEvent"/> class.
        /// </summary>
        /// <param name="conditionSet"></param>
        /// <param name="actionSet"></param>
        public CollisionItemEvent(IList<string> conditionSet, IList<string> actionSet)
            : base(conditionSet, actionSet)
        {
            var isTypeCondition = this.Conditions.FirstOrDefault(func => IsTypeFunctionName.Equals(func.FunctionName));

            if (isTypeCondition == null)
            {
                throw new ArgumentNullException($"Unable to find {IsTypeFunctionName} function.");
            }

            this.ThingIdOfCollision = Convert.ToUInt16(isTypeCondition.Parameters[1]);
        }

        public ushort ThingIdOfCollision { get; }

        public override ItemEventType Type => ItemEventType.Collision;
    }
}