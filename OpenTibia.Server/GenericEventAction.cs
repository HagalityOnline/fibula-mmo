// <copyright file="GenericEventAction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server
{
    using System;
    using OpenTibia.Common.Helpers;
    using OpenTibia.Scheduling.Contracts.Abstractions;

    internal class GenericEventAction : IEventAction
    {
        private readonly Action action;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEventAction"/> class.
        /// </summary>
        /// <param name="action"></param>
        public GenericEventAction(Action action)
        {
            action.ThrowIfNull();

            this.action = action;
        }

        public void Execute()
        {
            this.action();
        }
    }
}