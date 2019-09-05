// <copyright file="IOpenTibiaListener.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Contracts.Abstractions
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Common interface of all open tibia listeners.
    /// </summary>
    public interface IOpenTibiaListener
    {
        /// <summary>
        /// Begins listening for requests.
        /// </summary>
        /// <param name="cancellationToken">A token to observe for cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous listening operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}