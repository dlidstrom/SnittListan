﻿#nullable enable

using Castle.MicroKernel;
using EventStoreLite;
using Postal;
using Snittlistan.Web.Infrastructure.Bits;
using Snittlistan.Web.Infrastructure.Database;
using System.Data.Entity;

namespace Snittlistan.Web.Infrastructure;

public record CompositionRoot(
    IKernel Kernel,
    Raven.Client.IDocumentStore DocumentStore,
    Raven.Client.IDocumentSession DocumentSession,
    IEventStoreSession EventStoreSession,
    Databases Databases,
    EventStore EventStore,
    IEmailService EmailService,
    IBitsClient BitsClient)
{
    public Guid CorrelationId
    {
        get
        {
            if (CurrentHttpContext.Instance().Items["CorrelationId"] is Guid correlationId)
            {
                return correlationId;
            }

            correlationId = Guid.NewGuid();
            CurrentHttpContext.Instance().Items["CorrelationId"] = correlationId;
            return correlationId;
        }
    }

    public async Task<Tenant> GetCurrentTenant()
    {
        string hostname = CurrentHttpContext.Instance().Request.ServerVariables["SERVER_NAME"];
        Tenant? loadedTenant = Databases.Snittlistan.Tenants.Local.SingleOrDefault(x => x.Hostname == hostname);
        if (loadedTenant != null)
        {
            return loadedTenant;
        }

        Tenant? tenant = await Databases.Snittlistan.Tenants.SingleOrDefaultAsync(x => x.Hostname == hostname);
        if (tenant == null)
        {
            throw new Exception($"No tenant found for hostname '{hostname}'");
        }

        return tenant;
    }
}
