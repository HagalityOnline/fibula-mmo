// <copyright file="IPlayer.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data.Contracts
{
    public interface ICipPlayer
    {
        short Player_Id { get; }

        string Charname { get; }

        int Account_Id { get; }

        int Account_Nr { get; }

        int Creation { get; }

        int Lastlogin { get; }

        byte Gender { get; }

        byte Online { get; }

        string Vocation { get; }

        byte Hideprofile { get; }

        int Playerdelete { get; }

        short Level { get; }

        string Residence { get; }

        string Oldname { get; }

        string Comment { get; }

        string CharIp { get; }
    }
}
