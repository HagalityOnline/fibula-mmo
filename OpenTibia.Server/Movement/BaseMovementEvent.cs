// <copyright file="BaseMovementEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Server.Movement
{
    using OpenTibia.Scheduling;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using OpenTibia.Server.Contracts.Abstractions;
    using OpenTibia.Server.Notifications;

    /// <summary>
    /// Class that represents a common base bewteen movements.
    /// </summary>
    internal abstract class BaseMovementEvent : BaseEvent
    {
        /// <summary>
        /// Caches the requestor creature, if defined.
        /// </summary>
        private ICreature requestor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMovementEvent"/> class.
        /// </summary>
        /// <param name="requestorId">The id of the creature requesting the movement.</param>
        /// <param name="evaluationTime">The time to evaluate the movement.</param>
        protected BaseMovementEvent(uint requestorId, EvaluationTime evaluationTime)
            : base(requestorId, evaluationTime)
        {
            this.ActionsOnFail.Add(
                new GenericEventAction(
                    () =>
                    {
                        if (this.Requestor is Player player)
                        {
                            Game.Instance.NotifySinglePlayer(player, conn =>
                                new GenericNotification(
                                    conn,
                                    new PlayerWalkCancelPacket { Direction = player.ClientSafeDirection },
                                    new TextMessagePacket { Message = this.ErrorMessage ?? "Sorry, not possible.", Type = MessageType.StatusSmall }));
                        }
                    }));
        }

        /// <summary>
        /// Gets the creature that is requesting the event, if known.
        /// </summary>
        public ICreature Requestor
        {
            get
            {
                if (this.requestor == null)
                {
                    this.requestor = this.RequestorId == 0 ? null : Game.Instance.GetCreatureWithId(this.RequestorId);
                }

                return this.requestor;
            }
        }
    }
}
