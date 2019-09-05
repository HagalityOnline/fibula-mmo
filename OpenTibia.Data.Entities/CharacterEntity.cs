// -----------------------------------------------------------------
// <copyright file="CharacterEntity.cs" company="2Dudes">
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
    /// Class that represents a character entity.
    /// </summary>
    public class CharacterEntity : BaseEntity, ICharacterEntity
    {
        public string Name { get; set; }

        public Guid AccountId { get; set; }

        public string Vocation { get; set; }

        public short Level { get; set; }

        public byte Gender { get; set; }

        public DateTimeOffset Creation { get; set; }

        public DateTimeOffset LastLogin { get; set; }
    }
}