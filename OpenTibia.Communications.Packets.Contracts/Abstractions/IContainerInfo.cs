﻿// -----------------------------------------------------------------
// <copyright file="IContainerInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Interface that represents a container move up request info.
    /// </summary>
    public interface IContainerInfo
    {
        /// <summary>
        /// Gets the id of the container.
        /// </summary>
        byte ContainerId { get; }
    }
}
