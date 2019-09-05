// -----------------------------------------------------------------
// <copyright file="ItemHolderChangeEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using OpenTibia.Server.Contracts.Abstractions;

    public delegate void ItemHolderChangeEvent(IItem itemChanged, uint oldHolderId);
}
