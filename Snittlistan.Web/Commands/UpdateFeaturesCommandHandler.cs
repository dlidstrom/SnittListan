﻿#nullable enable

using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Database;
using System.Data.Entity;

namespace Snittlistan.Web.Commands;

public class UpdateFeaturesCommandHandler : CommandHandler<UpdateFeaturesCommandHandler.Command>
{
    public override async Task Handle(HandlerContext<Command> context)
    {
        Tenant tenant = await CompositionRoot.GetCurrentTenant();
        KeyValueProperty? settingsProperty =
            await CompositionRoot.Databases.Snittlistan.KeyValueProperties.SingleOrDefaultAsync(
                x => x.Key == TenantFeatures.Key && x.TenantId == tenant.TenantId);

        if (settingsProperty == null)
        {
            settingsProperty = CompositionRoot.Databases.Snittlistan.KeyValueProperties.Add(
                new(
                    tenant.TenantId,
                    TenantFeatures.Key,
                    new TenantFeatures(context.Payload.RosterMailEnabled)));
        }
        else
        {
            settingsProperty.ModifyValue<TenantFeatures>(
                x => x with
                {
                    RosterMailEnabled = context.Payload.RosterMailEnabled
                });
        }
    }

    public record Command(
        bool RosterMailEnabled);
}
