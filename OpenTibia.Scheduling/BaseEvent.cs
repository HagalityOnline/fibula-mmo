// <copyright file="BaseEvent.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using OpenTibia.Scheduling.Contracts.Abstractions;
    using OpenTibia.Scheduling.Contracts.Enumerations;
    using Priority_Queue;

    /// <summary>
    /// Abstract class that represents the base event for scheduling.
    /// </summary>
    public abstract class BaseEvent : FastPriorityQueueNode, IEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="evaluationTime">Optional. The time at which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnExecute"/>.</param>
        public BaseEvent(EvaluationTime evaluationTime = EvaluationTime.OnExecute)
        {
            this.EventId = Guid.NewGuid().ToString("N");
            this.RequestorId = 0;
            this.EvaluateAt = evaluationTime;

            this.Conditions = new List<IEventCondition>();
            this.ActionsOnPass = new List<IEventAction>();
            this.ActionsOnFail = new List<IEventAction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="requestorId">Optional. The id of the creature or entity requesting the event. Default is 0.</param>
        /// <param name="evaluationTime">Optional. The time at which the event's conditions should be evaluated. Default is <see cref="EvaluationTime.OnExecute"/>.</param>
        public BaseEvent(uint requestorId = 0, EvaluationTime evaluationTime = EvaluationTime.OnExecute)
            : this(evaluationTime)
        {
            this.RequestorId = requestorId;
        }

        /// <summary>
        /// Gets a unique identifier for this event.
        /// </summary>
        public string EventId { get; }

        /// <summary>
        /// Gets the id of the requestor of this event, if available.
        /// </summary>
        public uint RequestorId { get; }

        /// <summary>
        /// Gets or sets the error message that should be bubbled back to the player if the event cannot be executed.
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Gets a <see cref="EvaluationTime"/> value indicating when this event should be evaluated.
        /// </summary>
        public EvaluationTime EvaluateAt { get; }

        /// <summary>
        /// Gets a value indicating whether the event can be executed.
        /// </summary>
        public bool CanBeExecuted
        {
            get
            {
                var allPassed = true;

                foreach (var policy in this.Conditions)
                {
                    allPassed &= policy.Evaluate();

                    if (!allPassed)
                    {
                        // TODO: proper logging.
                        Console.WriteLine($"Failed event condition {policy.GetType().Name}.");
                        this.ErrorMessage = policy.ErrorMessage;
                        break;
                    }
                }

                return allPassed;
            }
        }

        /// <inheritdoc/>
        public IList<IEventCondition> Conditions { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnPass { get; }

        /// <inheritdoc/>
        public IList<IEventAction> ActionsOnFail { get; }

        /// <summary>
        /// Executes the event. Performs the <see cref="ActionsOnPass"/> on the <see cref="ActionsOnFail"/> depending if the conditions were met.
        /// </summary>
        public void Process()
        {
            if (this.EvaluateAt == EvaluationTime.OnSchedule || this.CanBeExecuted)
            {
                int i = 1;
                foreach (var action in this.ActionsOnPass)
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    action.Execute();
                    sw.Stop();

                    Console.WriteLine($"Executed ({i++} of {this.ActionsOnPass.Count})... done in {sw.Elapsed}.");
                }

                return;
            }

            foreach (var action in this.ActionsOnFail)
            {
                action.Execute();
            }
        }
    }
}