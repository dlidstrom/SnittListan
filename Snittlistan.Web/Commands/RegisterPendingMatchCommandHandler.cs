﻿#nullable enable

using Elmah;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Indexes;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Bits;
using Snittlistan.Web.Models;

namespace Snittlistan.Web.Commands;

public class RegisterPendingMatchCommandHandler : CommandHandler<RegisterPendingMatchCommandHandler.Command>
{
    public override async Task Handle(HandlerContext<Command> context)
    {
        WebsiteConfig websiteConfig = CompositionRoot.DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
        Player[] players =
            CompositionRoot.DocumentSession.Query<Player, PlayerSearch>()
                .ToArray()
                .Where(x => x.PlayerItem?.LicNbr != null)
                .ToArray();
        Roster pendingMatch = CompositionRoot.DocumentSession.Load<Roster>(context.Payload.RosterId);
        try
        {
            BitsParser parser = new(players);
            BitsMatchResult bitsMatchResult = await CompositionRoot.BitsClient.GetBitsMatchResult(pendingMatch.BitsMatchId);
            if (bitsMatchResult.HeadInfo.MatchFinished == false)
            {
                Log.Info($"Match {pendingMatch.BitsMatchId} not yet finished");
                return;
            }

            if (pendingMatch.IsFourPlayer)
            {
                Parse4Result? parse4Result = parser.Parse4(bitsMatchResult, websiteConfig.ClubId);
                if (parse4Result != null)
                {
                    if (parse4Result.Series.Length != 4 && parse4Result.Turn <= 20)
                    {
                        Log.Info($"detected unfinished match: {parse4Result.Series.Length} series have been registered");
                        return;
                    }

                    List<string> allPlayerIds = parse4Result.GetPlayerIds();
                    pendingMatch.SetPlayers(allPlayerIds);
                    await context.ExecuteCommand(
                        new RegisterMatch4CommandHandler.Command(pendingMatch.Id!, parse4Result, null, null));
                }
            }
            else
            {
                ParseResult? parseResult = parser.Parse(bitsMatchResult, websiteConfig.ClubId);
                if (parseResult != null)
                {
                    if (parseResult.Series.Length != 4 && parseResult.Turn <= 20)
                    {
                        Log.Info($"detected unfinished match: {parseResult.Series.Length} series have been registered");
                        return;
                    }

                    List<string> allPlayerIds = parseResult.GetPlayerIds();
                    pendingMatch.SetPlayers(allPlayerIds);
                    await context.ExecuteCommand(
                        new RegisterMatchCommandHandler.Command(pendingMatch.Id!, parseResult));
                }
            }
        }
        catch (Exception e)
        {
            ErrorSignal
                .FromCurrentContext()
                .Raise(new Exception($"Unable to auto register match {pendingMatch.Id} ({pendingMatch.BitsMatchId})", e));
            Log.Warn(e);
        }
    }

    public record Command(string RosterId);
}
