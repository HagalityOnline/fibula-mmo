﻿// -----------------------------------------------------------------
// <copyright file="OnContentUpdated.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts
{
    using OpenTibia.Server.Contracts.Abstractions;

    public delegate void OnContentUpdated(IContainer container, byte index, IItem item);
}