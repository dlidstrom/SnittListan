﻿namespace Snittlistan.Queue.Messages
{
    using System;

    public class InitiateUpdateMailTask : ITask
    {
        public InitiateUpdateMailTask(string rosterId, int rosterVersion, Guid correlationId)
        {
            RosterId = rosterId;
            CorrelationId = correlationId;
        }

        public string RosterId { get; }

        public Guid CorrelationId { get; }

        public BusinessKey BusinessKey => new(GetType(), RosterId);
    }
}