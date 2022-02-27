﻿#nullable enable

using Snittlistan.Web.Infrastructure;
using System.Reflection;

namespace Snittlistan.Web.Commands;

public class CommandExecutor
{
    private readonly CompositionRoot compositionRoot;
    private readonly Guid correlationId;
    private readonly Guid? causationId;
    private readonly string createdBy;

    public CommandExecutor(
        CompositionRoot compositionRoot,
        Guid correlationId,
        Guid? causationId,
        string createdBy)
    {
        this.compositionRoot = compositionRoot;
        this.correlationId = correlationId;
        this.causationId = causationId;
        this.createdBy = createdBy;
    }

    public async Task Execute<TCommand>(TCommand command)
        where TCommand : class
    {
        Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        MethodInfo handleMethod = handlerType.GetMethod(nameof(ICommandHandler<TCommand>.Handle));
        object handler = compositionRoot.Kernel.Resolve(handlerType);
        TaskPublisher taskPublisher = new(
            compositionRoot.CurrentTenant,
            compositionRoot.Databases,
            correlationId,
            causationId);
        IHandlerContext handlerContext = (IHandlerContext)Activator.CreateInstance(
            typeof(HandlerContext<>).MakeGenericType(command.GetType()),
            compositionRoot,
            command,
            compositionRoot.CurrentTenant,
            correlationId,
            causationId);
        handlerContext.PublishMessage = async (task, publishDate) =>
        {
            if (publishDate != null)
            {
                await taskPublisher.PublishDelayedTask(task, publishDate.Value, createdBy);
            }
            else
            {
                await taskPublisher.PublishTask(task, createdBy);
            }
        };
        Task task = (Task)handleMethod.Invoke(handler, new[] { handlerContext });
        await task;
        _ = compositionRoot.Databases.Snittlistan.ChangeLogs.Add(new(
            compositionRoot.CurrentTenant.TenantId,
            correlationId,
            causationId,
            command,
            createdBy));
    }
}
