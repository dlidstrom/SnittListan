﻿#nullable enable

using Snittlistan.Queue.Messages;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Queries;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Models;

namespace Snittlistan.Web.TaskHandlers;

public class RegisterMatchesTaskHandler : TaskHandler<RegisterMatchesTask>
{
    public override Task Handle(HandlerContext<RegisterMatchesTask> context)
    {
        WebsiteConfig websiteConfig = CompositionRoot.DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
        Roster[] pendingMatches = ExecuteQuery(new GetPendingMatchesQuery(websiteConfig.SeasonId));
        foreach (Roster pendingMatch in pendingMatches.Where(x => x.SkipRegistration == false))
        {
            context.PublishMessage(new RegisterMatchTask(pendingMatch.Id!, pendingMatch.BitsMatchId));
        }

        return Task.CompletedTask;
    }
}
