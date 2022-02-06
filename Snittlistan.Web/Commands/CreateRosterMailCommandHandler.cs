﻿#nullable enable

using Snittlistan.Queue.Messages;
using Snittlistan.Web.Infrastructure;
using System.Data.Entity;

namespace Snittlistan.Web.Commands;

public class CreateRosterMailCommandHandler : CommandHandler<CreateRosterMailCommandHandler.Command>
{
    public override async Task Handle(HandlerContext<Command> context)
    {
        IQueryable<string> query =
            from rosterMail in CompositionRoot.Databases.Snittlistan.RosterMails
            where rosterMail.RosterKey == context.Payload.RosterKey
                && rosterMail.PublishedDate == null
            select rosterMail.RosterKey;
        string[] rosterIds = await query.ToArrayAsync();
        if (rosterIds.Any() == false)
        {
            _ = CompositionRoot.Databases.Snittlistan.RosterMails.Add(new(context.Payload.RosterKey));
            PublishRosterMailsTask task = new(
                context.Payload.RosterKey,
                context.Payload.RosterLink);
            context.PublishMessage(task, DateTime.Now.AddSeconds(10)); // TODO revert to 10 minutes
        }
    }

    public record Command(string RosterKey, Uri RosterLink);
}
