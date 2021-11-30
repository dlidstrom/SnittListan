﻿#nullable enable

namespace Snittlistan.Web.Areas.V2.Tasks
{
    using System.Threading.Tasks;
    using Snittlistan.Queue.Messages;
    using Snittlistan.Web.Infrastructure;
    using Snittlistan.Web.Infrastructure.Indexes;
    using Snittlistan.Web.Models;

    public class InitializeIndexesTaskHandler : TaskHandler<InitializeIndexesTask>
    {
        public override Task Handle(MessageContext<InitializeIndexesTask> context)
        {
            IndexCreator.CreateIndexes(DocumentStore);
            User admin = DocumentSession.Load<User>(User.AdminId);
            if (admin == null)
            {
                admin = new("", "", context.Task.Email, context.Task.Password)
                {
                    Id = User.AdminId
                };
                admin.Initialize(t => context.PublishMessageDelegate(t));
                admin.Activate();
                DocumentSession.Store(admin);
            }
            else
            {
                admin.SetEmail(context.Task.Email);
                admin.SetPassword(context.Task.Password);
                admin.Activate();
            }

            return Task.CompletedTask;
        }
    }
}
