// -----------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Contracts.Abstractions
{
    using System;
    using OpenTibia.Data.Repositories.Contracts.Abstractions;

    /// <summary>
    /// Interface for units of work.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository of accounts.
        /// </summary>
        IAccountRepository Accounts { get; }

        /// <summary>
        /// Gets the repository of characters.
        /// </summary>
        ICharacterRepository Characters { get; }

        /// <summary>
        /// Saves all changes made during this unit of work to the persistent store.
        /// </summary>
        /// <returns>The number of changes saved.</returns>
        int Complete();
    }
}
