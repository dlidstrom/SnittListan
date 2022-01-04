﻿using Snittlistan.Queue.Messages;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Infrastructure;

#nullable enable

namespace Snittlistan.Web.Areas.V2.Tasks;
public class InitiateUpdateMailTaskHandler : TaskHandler<InitiateUpdateMailTask>
{
    public override Task Handle(MessageContext<InitiateUpdateMailTask> context)
    {
        Roster roster = DocumentSession.Load<Roster>(context.Task.RosterId);
        AuditLogEntry auditLogEntry = roster.AuditLogEntries.Single(x => x.CorrelationId == context.CorrelationId);
        RosterState before = (RosterState)auditLogEntry.Before;
        RosterState after = (RosterState)auditLogEntry.After;
        IEnumerable<string> affectedPlayers = before.Players.Concat(after.Players);
        foreach (string playerId in new HashSet<string>(affectedPlayers))
        {
            SendUpdateMailTask message = new(
                context.Task.RosterId,
                playerId);
            context.PublishMessageDelegate(message);
        }

        return Task.CompletedTask;
    }
}
