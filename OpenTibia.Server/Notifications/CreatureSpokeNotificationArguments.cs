// <copyright file="CreatureSpokeNotificationArguments.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Notifications
{
    using OpenTibia.Common.Helpers;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Contracts.Enumerations;

    internal class CreatureSpokeNotificationArguments : INotificationArguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatureSpokeNotificationArguments"/> class.
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="speechType"></param>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        public CreatureSpokeNotificationArguments(ICreature creature, SpeechType speechType, string message, ChatChannelType channel = ChatChannelType.None)
        {
            creature.ThrowIfNull(nameof(creature));
            message.ThrowIfNullOrWhiteSpace(nameof(message));

            this.Creature = creature;
            this.SpeechType = speechType;
            this.Message = message;
            this.Channel = channel;
        }

        public ICreature Creature { get; }

        public SpeechType SpeechType { get; }

        public string Message { get; }

        public ChatChannelType Channel { get; }
    }
}