﻿// -----------------------------------------------------------------
// <copyright file="KeyVaultSecretsProviderOptions.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Author: Jose L. Nunez de Caceres
// http://linkedin.com/in/jlnunez89
//
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Providers.Azure
{
    /// <summary>
    /// Class that represents options for the Azure Key Vault secrets provider configuration.
    /// </summary>
    public class KeyVaultSecretsProviderOptions
    {
        /// <summary>
        /// Gets or sets the base url for the Key Vault.
        /// </summary>
        public string VaultBaseUrl { get; set; }
    }
}
