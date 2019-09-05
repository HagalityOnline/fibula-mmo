// -----------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities
{
    using System;
    using OpenTibia.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Abstract class that represents the base of all entities.
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        /// <summary>
        /// Gets or sets the id of this entity.
        /// </summary>
        public Guid Id { get; set; }
    }
}