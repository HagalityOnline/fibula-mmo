// <copyright file="IItemLoader.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    using System.Collections.Generic;

    public interface IItemLoader
    {
        Dictionary<ushort, IItemType> Load(string objectsFileName);
    }
}
