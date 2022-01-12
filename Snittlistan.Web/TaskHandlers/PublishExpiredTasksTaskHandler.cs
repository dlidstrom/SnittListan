﻿#nullable enable

using Snittlistan.Queue.Messages;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Database;
using System.Data.Entity;

namespace Snittlistan.Web.TaskHandlers;

public class PublishExpiredTasksTaskHandler : TaskHandler<PublishExpiredTasksTask>
{
    public override async Task Handle(HandlerContext<PublishExpiredTasksTask> context)
    {
        DateTime now = DateTime.Now;
        IQueryable<DelayedTask> query =
            from task in CompositionRoot.Databases.Snittlistan.DelayedTasks
            where task.PublishDate < now
            select task;
        DelayedTask[] expiredTasks = await query.ToArrayAsync();
        foreach (DelayedTask task in expiredTasks)
        {
            context.PublishMessage(task.Task);
            task.MarkAsPublished(now);
        }
    }
}
