﻿#nullable enable

using Snittlistan.Web.Commands;
using Snittlistan.Web.Infrastructure.Database;

namespace Snittlistan.Web.Infrastructure;

public class HandlerContext<TPayload> : IHandlerContext
{
    private readonly CompositionRoot compositionRoot;

    public HandlerContext(
        CompositionRoot compositionRoot,
        Databases databases,
        TPayload payload,
        Tenant tenant,
        Guid correlationId,
        Guid causationId)
    {
        this.compositionRoot = compositionRoot;
        Databases = databases;
        Payload = payload;
        Tenant = tenant;
        CorrelationId = correlationId;
        CausationId = causationId;
    }

    public Databases Databases { get; }

    public TPayload Payload { get; }

    public Tenant Tenant { get; }

    public Guid CorrelationId { get; }

    public Guid CausationId { get; }

    public Guid MessageId { get; }

    public PublishMessageDelegate PublishMessage { get; set; } = null!;

    public async Task ExecuteCommand<TCommand>(TCommand command)
        where TCommand : class
    {
        CommandExecutor commandExecutor = new(
            compositionRoot,
            Databases,
            CorrelationId,
            CausationId,
            "system");
        await commandExecutor.Execute(command);
    }
}
