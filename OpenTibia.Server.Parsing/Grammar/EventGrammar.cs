// <copyright file="EventGrammar.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Parsing.Grammar
{
    using System.Linq;
    using OpenTibia.Common.Helpers;
    using Sprache;

    public class EventGrammar
    {
        public static readonly Parser<MoveUseEvent> Event =
            from rule in CipGrammar.ConditionalActionRule
            select new MoveUseEvent(rule);

        public class MoveUseEvent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MoveUseEvent"/> class.
            /// </summary>
            /// <param name="rule"></param>
            public MoveUseEvent(ConditionalActionRule rule)
            {
                rule.ThrowIfNull(nameof(rule));

                this.Type = rule.ConditionSet.FirstOrDefault();
                this.Rule = rule;

                rule.ConditionSet.RemoveAt(0); // remove first.
            }

            public string Type { get; }

            public ConditionalActionRule Rule { get; }
        }
    }
}
