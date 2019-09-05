// -----------------------------------------------------------------
// <copyright file="LoginFailureReason.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Data.Contracts.Enumerations
{
    public enum LoginFailureReason : byte
    {
        None = 0x00,
        WrongClientVersion = 0x01,
        // 0x02 = doesnt exist
        // 0x03 = doesnt live on this world
        // 0x04 = Private
        // 0x05 = ???
        AccountOrPasswordIncorrect = 0x06,
        // 0x07 = Account disabled for 5 minutes
        // 0x08 = 0x06
        // 0x09 = IP Address blocked for 30 minutes
        Bannished = 0x0A,
        // 0x0B = Banished for name
        // 0x0C = IP Banned
        AnotherCharacterIsLoggedIn = 0x0D,
        // 0x0E = May only login with GM acc
        InternalServerError = 0x0F,
    }
}
