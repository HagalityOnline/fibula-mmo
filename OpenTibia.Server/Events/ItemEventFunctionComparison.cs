// <copyright file="ItemEventFunctionComparison.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Events
{
    using OpenTibia.Data.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;

    internal class ItemEventFunctionComparison : IItemEventFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEventFunctionComparison"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="compareIdentifier"></param>
        /// <param name="parameters"></param>
        public ItemEventFunctionComparison(string name, FunctionComparisonType type, string compareIdentifier, object[] parameters)
        {
            this.FunctionName = name;
            this.Type = type;
            this.CompareToIdentifier = compareIdentifier;
            this.Parameters = parameters;
        }

        public string FunctionName { get; }

        public object[] Parameters { get; }

        public FunctionComparisonType Type { get; }

        public string CompareToIdentifier { get; }
    }
}