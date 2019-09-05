// -----------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data
{
    using OpenTibia.Data.Contracts.Abstractions;
    using OpenTibia.Data.Repositories.Contracts.Abstractions;

    /// <summary>
    /// Class that represents a unit of work.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// A reference to the underlying context on this unit of work.
        /// </summary>
        private readonly OpenTibiaContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context to work on.</param>
        public UnitOfWork(OpenTibiaContext context)
        {
            this.context = context;

            this.Accounts = new AccountRepository(context);
            this.Characters = new CharacterRepository(context);
        }

        /// <summary>
        /// Gets a reference to the accounts repository.
        /// </summary>
        public IAccountRepository Accounts { get; }

        /// <summary>
        /// Gets a reference to the characters repository.
        /// </summary>
        public ICharacterRepository Characters { get; }

        /// <summary>
        /// Completes this unit of work.
        /// </summary>
        /// <returns>The number of changes saved upon completion of this unit of work.</returns>
        public int Complete()
        {
            return this.context.SaveChanges();
        }

        /// <summary>
        /// Disposes this unit of work and it's resources.
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}