// -----------------------------------------------------------------
// <copyright file="AccountEntity.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Entities
{
    using System;
    using System.Net;
    using OpenTibia.Data.Entities.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a character entity.
    /// </summary>
    public class AccountEntity : BaseEntity, IAccountEntity
    {
        public uint Number { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTimeOffset Creation { get; set; }

        public IPAddress CreationIp { get; set; }

        public IPAddress LastLoginIp { get; set; }

        public DateTimeOffset LastLogin { get; set; }

        public IPAddress SessionIp { get; set; }

        public ushort PremiumDays { get; set; }

        public ushort TrialOrBonusPremiumDays { get; set; }

        public byte AccessLevel { get; set; }

        public bool Premium { get; set; }

        public bool TrialPremium { get; set; }

        public bool Banished { get; set; }

        public DateTimeOffset BanishedUntil { get; set; }

        public bool Deleted { get; set; }
    }
}