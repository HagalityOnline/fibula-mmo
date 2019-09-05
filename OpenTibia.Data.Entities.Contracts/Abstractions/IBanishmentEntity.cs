// -----------------------------------------------------------------
// <copyright file="IBanishmentEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities.Contracts.Abstractions
{
    using System;
    using System.Net;

    /// <summary>
    /// Interface for banishment entities.
    /// </summary>
    public interface IBanishmentEntity : IIdentifiableEntity
    {
        Guid AccountId { get; }

        Guid GamemasterId { get; }

        IPAddress IpAddress { get; }

        string Violation { get; }

        string Comment { get; }

        DateTimeOffset Timestamp { get; }

        DateTimeOffset BanishedUntil { get; }

        byte PunishmentType { get; }
    }
}
