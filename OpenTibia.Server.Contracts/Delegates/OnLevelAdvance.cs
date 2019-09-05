// -----------------------------------------------------------------
// <copyright file="OnLevelAdvance.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using OpenTibia.Server.Contracts.Enumerations;

    /// <summary>
    /// Delegate meant for skill advancement.
    /// </summary>
    /// <param name="skillType">The type of skill that advanced.</param>
    public delegate void OnLevelAdvance(SkillType skillType);
}
