// <copyright file="Speech.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Contracts.Structs
{
    using OpenTibia.Server.Contracts.Enumerations;

    public struct Speech
    {
        public SpeechType Type { get; set; }

        public string Receiver { get; set; }

        public string Message { get; set; }

        public ChatChannelType ChannelId { get; set; }
    }
}