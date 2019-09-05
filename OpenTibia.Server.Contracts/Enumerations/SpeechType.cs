// -----------------------------------------------------------------
// <copyright file="SpeechType.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------

namespace OpenTibia.Server.Contracts.Enumerations
{
    public enum SpeechType : byte
    {
        /// <summary>
        /// Normal speech.
        /// </summary>
        Normal = 0x01,

        /// <summary>
        /// Whispering (#w).
        /// </summary>
        Whisper = 0x02,

        /// <summary>
        /// Yelling (#y).
        /// </summary>
        Yell = 0x03,

        /// <summary>
        /// Players speaking privately to players.
        /// </summary>
        Private = 0x04,

        /// <summary>
        /// Yellow message in chat.
        /// </summary>
        ChannelYellow = 0x05,

        /// <summary>
        /// Reporting rule violation (Ctrl + R).
        /// </summary>
        RuleViolationReport = 0x06,

        /// <summary>
        /// Answering report.
        /// </summary>
        RuleViolationAnswer = 0x07,

        /// <summary>
        /// Answering the answer of the report.
        /// </summary>
        RuleViolationContinue = 0x08,

        /// <summary>
        /// Broadcast a message (#b).
        /// </summary>
        Broadcast = 0x09,

        // ChannelRed = 0x05,
        // PrivateRed = 0x04,   //Red private - @name@ text
        // ChannelOrange = 0x05,    //Talk orange on text
        // ChannelRedAnonymous = 0x05,  //Talk red anonymously on chat - #d

        /// <summary>
        /// Orange text from monsters.
        /// </summary>
        MonsterSay = 0x0E,

        // MonsterYell = 0x0E,
    }
}
