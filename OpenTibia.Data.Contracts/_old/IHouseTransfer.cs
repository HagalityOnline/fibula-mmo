// <copyright file="IHouseTransfer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IHouseTransfer
    {
        long Id { get; }

        short HouseId { get; }

        int TransferTo { get; }

        long Gold { get; }

        byte Done { get; }
    }
}
