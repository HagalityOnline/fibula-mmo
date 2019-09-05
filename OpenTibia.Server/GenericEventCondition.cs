// <copyright file="GenericEventCondition.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Scheduling.Contracts.Abstractions;

    internal class GenericEventCondition : IEventCondition
    {
        private readonly Func<bool> condition;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEventCondition"/> class.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="errorMsg"></param>
        public GenericEventCondition(Func<bool> condition, string errorMsg = "")
        {
            condition.ThrowIfNull();

            this.condition = condition;
            this.ErrorMessage = errorMsg;
        }

        public string ErrorMessage { get; }

        public bool Evaluate()
        {
            return this.condition();
        }
    }
}