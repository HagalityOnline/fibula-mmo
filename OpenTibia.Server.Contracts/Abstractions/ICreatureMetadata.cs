// <copyright file="ICreatureMetadata.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Abstractions
{
    public interface ICreatureMetadata
    {
        string Article { get; }

        string Name { get; }

        ushort MaxHitpoints { get; }

        ushort MaxManapoints { get; }

        ushort Corpse { get; }
    }
}
