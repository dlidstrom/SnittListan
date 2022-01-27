﻿#nullable enable

using Castle.Core.Logging;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Database;
using Snittlistan.Web.Models;
using System.Data.Entity;

namespace Snittlistan.Web.Commands;

public abstract class HandleMailCommandHandler<TCommand, TEmail>
    : ICommandHandler<TCommand>
    where TEmail : EmailBase
{
    public CompositionRoot CompositionRoot { get; set; } = null!;

    public ILogger Logger { get; set; } = NullLogger.Instance;

    public async Task Handle(HandlerContext<TCommand> context)
    {
        string key = GetKey(context.Payload);
        RateLimit? rateLimit = await CompositionRoot.Databases.Snittlistan.RateLimits
            .SingleOrDefaultAsync(x => x.Key == key);
        if (rateLimit == null)
        {
            (int rate, int perSeconds) = GetRate(context.Payload);
            rateLimit = CompositionRoot.Databases.Snittlistan.RateLimits.Add(
                new(key, 1, rate, perSeconds));
        }

        DateTime now = DateTime.Now;
        rateLimit.UpdateAllowance(now);
        if (rateLimit.Allowance < 1)
        {
            throw new HandledException($"allowance = {rateLimit.Allowance:N2}, wait to reach 1");
        }

        TEmail email = await CreateEmail(context);
        EmailState state = email.State;
        _ = CompositionRoot.Databases.Snittlistan.SentEmails.Add(new(
            email.From,
            email.To,
            email.Bcc,
            email.Subject,
            state));
        rateLimit.DecreaseAllowance(now);
        int changesSaved = await CompositionRoot.Databases.Snittlistan.SaveChangesAsync();
        if (changesSaved > 0)
        {
            Logger.InfoFormat(
                "saved {changesSaved} to database",
                changesSaved);
        }

        await CompositionRoot.EmailService.SendAsync(email);
    }

    protected abstract Task<TEmail> CreateEmail(HandlerContext<TCommand> context);

    protected abstract string GetKey(TCommand command);

    protected abstract RatePerSeconds GetRate(TCommand command);

    protected record RatePerSeconds(int Rate, int PerSeconds);
}
