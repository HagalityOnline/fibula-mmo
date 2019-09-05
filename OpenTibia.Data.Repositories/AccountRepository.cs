// -----------------------------------------------------------------
// <copyright file="AccountRepository.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data
{
    using Microsoft.EntityFrameworkCore;
    using OpenTibia.Data.Entities;
    using OpenTibia.Data.Repositories.Contracts.Abstractions;

    /// <summary>
    /// Class that represents an accounts repository.
    /// </summary>
    public class AccountRepository : Repository<AccountEntity>, IAccountRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountRepository"/> class.
        /// </summary>
        /// <param name="context">The context to initialize the repository with.</param>
        public AccountRepository(DbContext context)
            : base(context)
        {
        }
    }
}
