// -----------------------------------------------------------------
// <copyright file="ICreatureManager.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Interface for creature manager.
    /// </summary>
    public interface ICreatureManager : ICreatureFinder
    {
        /// <summary>
        /// Registers a new creature to the manager.
        /// </summary>
        /// <param name="creature">The creature to register.</param>
        void RegisterCreature(ICreature creature);

        /// <summary>
        /// Unregisters a creature from the manager.
        /// </summary>
        /// <param name="creature">The creature to unregister.</param>
        void UnregisterCreature(ICreature creature);
    }
}