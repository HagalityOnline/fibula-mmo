// <copyright file="House.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using OpenTibia.Data.Contracts;

    [Table("house", Schema = "opentibia_classic")]
    public class House : IAuction
    {
        [Key]
        public short HouseId { get; }

        public string HouseName { get; }

        public short RentOffset { get; }

        public short Area { get; }

        public byte GuildHouse { get; }

        public short Sqm { get; }

        public string Description { get; }

        public string Coords { get; }

        public int Price { get; }

        public int PriceOld { get; }

        public byte Auctioned { get; }

        public byte AuctionDays { get; }

        public int Bid { get; }

        public int BidderId { get; }

        public byte PricePerSqm { get; }

        public int Bidlimit { get; }
    }
}
