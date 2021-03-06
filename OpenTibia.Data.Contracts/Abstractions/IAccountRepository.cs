﻿// -----------------------------------------------------------------
// <copyright file="IAccountRepository.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Contracts.Abstractions
{
    using OpenTibia.Data.Entities;

    /// <summary>
    /// Interface for an accounts repository.
    /// </summary>
    public interface IAccountRepository : IRepository<AccountEntity>
    {
    }
}