// -----------------------------------------------------------------
// <copyright file="AttackType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of different attack types.
    /// </summary>
    public enum AttackType : byte
    {
        /// <summary>
        /// Physical attack type.
        /// </summary>
        Physical,

        /// <summary>
        /// Poison based attack type.
        /// </summary>
        Posion,

        /// <summary>
        /// Fire based attack type.
        /// </summary>
        Fire,

        /// <summary>
        /// Electricity based attack type.
        /// </summary>
        Electric,

        /// <summary>
        /// Draining attack type.
        /// </summary>
        Drain,
    }
}
