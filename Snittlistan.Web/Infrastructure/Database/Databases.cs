﻿#nullable enable

namespace Snittlistan.Web.Infrastructure.Database;

public class Databases : IDisposable
{
    public Databases(
        ISnittlistanContext snittlistanContext,
        IBitsContext bitsContext)
    {
        Snittlistan = snittlistanContext;
        Bits = bitsContext;
    }

    public ISnittlistanContext Snittlistan { get; }

    public IBitsContext Bits { get; }

    public void Dispose()
    {
        Snittlistan.Dispose();
        Bits.Dispose();
    }
}
