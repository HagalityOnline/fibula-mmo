// <copyright file="ItemEventFunction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using OpenTibia.Server.Contracts.Abstractions;

    internal class ItemEventFunction : IItemEventFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventFunction"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        public ItemEventFunction(string name, params object[] parameters)
        {
            this.FunctionName = name;
            this.Parameters = parameters;
        }

        public string FunctionName { get; }

        public object[] Parameters { get; }
    }
}