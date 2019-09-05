// -----------------------------------------------------------------
// <copyright file="ICharacterRepository.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Repositories.Contracts.Abstractions
{
    using OpenTibia.Data.Entities;

    /// <summary>
    /// Interface for a character repository.
    /// </summary>
    public interface ICharacterRepository : IRepository<CharacterEntity>
    {
    }
}