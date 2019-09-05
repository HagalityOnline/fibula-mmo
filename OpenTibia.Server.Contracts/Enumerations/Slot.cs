// -----------------------------------------------------------------
// <copyright file="Slot.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration of the possible slots.
    /// </summary>
    public enum Slot : byte
    {
        /// <summary>
        /// Uses both hands.
        /// </summary>
        TwoHanded = 0x00,

        /// <summary>
        /// The head slot.
        /// </summary>
        Head = 0x01,

        /// <summary>
        /// The necklace slot.
        /// </summary>
        Necklace = 0x02,

        /// <summary>
        /// The backpack slot.
        /// </summary>
        Backpack = 0x03,

        /// <summary>
        /// The chest slot.
        /// </summary>
        Body = 0x04,

        /// <summary>
        /// The right hand slot.
        /// </summary>
        Right = 0x05,

        /// <summary>
        /// The left hand slot.
        /// </summary>
        Left = 0x06,

        /// <summary>
        /// The pants slot.
        /// </summary>
        Legs = 0x07,

        /// <summary>
        /// The feet slot.
        /// </summary>
        Feet = 0x08,

        /// <summary>
        /// The ring slot.
        /// </summary>
        Ring = 0x09,

        /// <summary>
        /// The ammunition slot.
        /// </summary>
        Ammo = 0x0A,

        /// <summary>
        /// Special slot that is a wildcard.
        /// </summary>
        WhereEver = 0x0B,
    }
}
