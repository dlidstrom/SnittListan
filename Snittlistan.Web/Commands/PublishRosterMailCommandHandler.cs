﻿#nullable enable

using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Models;

namespace Snittlistan.Web.Commands;

public class PublishRosterMailCommandHandler
    : HandleMailCommandHandler<PublishRosterMailCommandHandler.Command, UpdateRosterEmail>
{
    protected override Task<UpdateRosterEmail> CreateEmail(HandlerContext<Command> context)
    {
        if (DateTime.Now.Second % 3 == 0)
        {
            throw new Exception("oh, no!");
        }

        Player player = CompositionRoot.DocumentSession.Load<Player>(context.Payload.PlayerId);
        Roster roster = CompositionRoot.DocumentSession
            .Include<Roster>(x => x.Players)
            .Load<Roster>(context.Payload.RosterId);
        FormattedAuditLog formattedAuditLog = roster.GetFormattedAuditLog(
            CompositionRoot.DocumentSession,
            context.CorrelationId);
        Player[] players = CompositionRoot.DocumentSession.Load<Player>(roster.Players);
        string teamLeader =
            roster.TeamLeader != null
            ? CompositionRoot.DocumentSession.Load<Player>(roster.TeamLeader).Name
            : string.Empty;
        UpdateRosterEmail email = new(
            player.Email,
            formattedAuditLog,
            players.Select(x => x.Name).ToArray(),
            teamLeader,
            context.Payload.ReplyToEmail,
            roster.Season,
            roster.Turn,
            context.Payload.RosterLink);
        return Task.FromResult(email);
    }

    protected override RatePerSeconds GetRate(HandlerContext<Command> context)
    {
        RatePerSeconds ratePerSeconds = new(
            $"roster_mail:{context.Payload.PlayerId}:{context.Tenant.TenantId}",
            1,
            600);
        return ratePerSeconds;
    }

    public record Command(
        string RosterId,
        string PlayerId,
        string ReplyToEmail,
        Uri RosterLink);
}