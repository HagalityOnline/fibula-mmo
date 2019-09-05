// <copyright file="IAuction.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IAuction
    {
        short HouseId { get; }

        string HouseName { get; }

        short RentOffset { get; }

        short Area { get; }

        byte GuildHouse { get; }

        short Sqm { get; }

        string Description { get; }

        string Coords { get; }

        int Price { get; }

        int PriceOld { get; }

        byte Auctioned { get; }

        byte AuctionDays { get; }

        int Bid { get; }

        int BidderId { get; }

        byte PricePerSqm { get; }

        int Bidlimit { get; }
    }
}
