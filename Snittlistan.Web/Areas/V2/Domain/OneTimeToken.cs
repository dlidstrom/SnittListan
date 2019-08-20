﻿namespace Snittlistan.Web.Areas.V2.Domain
{
    using System;
    using Raven.Abstractions;

    public class OneTimeToken
    {
        public OneTimeToken(string playerId)
        {
            PlayerId = playerId;
            CreatedDate = SystemTime.UtcNow;
        }

        public string Id { get; set; }

        public string PlayerId { get; private set; }

        public string OneTimeKey { get; private set; }

        public DateTimeOffset? UsedDate { get; private set; }

        public DateTimeOffset CreatedDate { get; private set; }

        public string Payload { get; private set; }

        public bool IsExpired()
        {
            var span = SystemTime.UtcNow - CreatedDate;
            return span.TotalDays > 1;
        }

        public void MarkUsed()
        {
            UsedDate = SystemTime.UtcNow;
        }

        public void Activate(Action<string> action, string payload)
        {
            OneTimeKey = Guid.NewGuid().ToString();
            Payload = payload;
            action.Invoke(OneTimeKey);
        }
    }
}