// <copyright file="IRandPlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface IRandPlayer
    {
        int RandId { get; }

        int AccountId { get; }

        int Order { get; }

        int AssignedTo { get; }
    }
}
