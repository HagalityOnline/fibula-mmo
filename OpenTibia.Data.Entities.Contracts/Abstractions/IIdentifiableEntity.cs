// -----------------------------------------------------------------
// <copyright file="IIdentifiableEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities.Contracts.Abstractions
{
    using System;

    /// <summary>
    /// Interface for all entities which contain an id.
    /// </summary>
    public interface IIdentifiableEntity : IEntity
    {
        /// <summary>
        /// Gets the id of this entity.
        /// </summary>
        Guid Id { get; }
    }
}
