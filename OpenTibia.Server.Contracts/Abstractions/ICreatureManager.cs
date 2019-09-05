// -----------------------------------------------------------------
// <copyright file="ICreatureManager.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server
{
    using System;
    using System.Collections.Generic;
    using OpenTibia.Server.Contracts.Abstractions;

    /// <summary>
    /// Interface for creature manager.
    /// </summary>
    public interface ICreatureManager
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

        /// <summary>
        /// Looks for a single creature with the id.
        /// </summary>
        /// <param name="creatureId">The creature id for which to look.</param>
        /// <returns>The creature instance, if found, and null otherwise.</returns>
        ICreature FindCreatureById(Guid creatureId);

        /// <summary>
        /// Gets all creatures known to this manager.
        /// </summary>
        /// <returns>A collection of creature instances.</returns>
        IEnumerable<ICreature> GetAllCreatures();
    }
}