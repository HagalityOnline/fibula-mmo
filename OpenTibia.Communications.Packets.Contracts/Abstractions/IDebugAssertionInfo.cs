// <copyright file="IDebugAssertionInfo.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Communications.Packets.Contracts.Abstractions
{
    /// <summary>
    /// Interface for player list information.
    /// </summary>
    public interface IDebugAssertionInfo
    {
        string AssertionLine { get; }

        string Date { get; }

        string Description { get; }

        string Comment { get; }
    }
}