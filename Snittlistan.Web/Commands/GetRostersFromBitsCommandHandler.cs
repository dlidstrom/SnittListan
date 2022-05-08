﻿#nullable enable

using Raven.Client;
using Snittlistan.Web.Areas.V2;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Indexes;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Bits.Contracts;
using Snittlistan.Web.Models;

namespace Snittlistan.Web.Commands;

public class GetRostersFromBitsCommandHandler
    : CommandHandler<GetRostersFromBitsCommandHandler.Command>
{
    public override async Task Handle(HandlerContext<Command> context)
    {
        WebsiteConfig websiteConfig = CompositionRoot.DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
        Logger.InfoFormat(
            "Importing BITS season {seasonId} for {teamFullName} (ClubId={clubId})",
            websiteConfig.SeasonId,
            CompositionRoot.CurrentTenant.TeamFullName,
            websiteConfig.ClubId);
        RosterSearchTerms.Result[] rosterSearchTerms =
            CompositionRoot.DocumentSession.Query<RosterSearchTerms.Result, RosterSearchTerms>()
                .Where(x => x.Season == websiteConfig.SeasonId)
                .Where(x => x.BitsMatchId != 0)
                .ProjectFromIndexFieldsInto<RosterSearchTerms.Result>()
                .ToArray();
        Roster[] rosters = CompositionRoot.DocumentSession.Load<Roster>(
            rosterSearchTerms.Select(x => x.Id));
        HashSet<int> foundMatchIds = new();

        // Team
        Logger.Info("Fetching teams");
        TeamResult[] teams = await CompositionRoot.BitsClient.GetTeam(
            websiteConfig.ClubId,
            websiteConfig.SeasonId);
        foreach (TeamResult teamResult in teams)
        {
            // Division
            Logger.Info("Fetching divisions");
            DivisionResult[] divisionResults = await CompositionRoot.BitsClient.GetDivisions(
                teamResult.TeamId,
                websiteConfig.SeasonId);

            // Match
            if (divisionResults.Length != 1)
            {
                throw new Exception($"Unexpected number of divisions: {divisionResults.Length}");
            }

            DivisionResult divisionResult = divisionResults[0];
            Logger.Info("Fetching match rounds");
            MatchRound[] matchRounds = await CompositionRoot.BitsClient.GetMatchRounds(
                teamResult.TeamId,
                divisionResult.DivisionId,
                websiteConfig.SeasonId);
            Dictionary<int, MatchRound> dict = matchRounds.ToDictionary(x => x.MatchId);
            foreach (int key in dict.Keys)
            {
                _ = foundMatchIds.Add(key);
            }

            // update existing rosters
            foreach (Roster roster in rosters.Where(x => dict.ContainsKey(x.BitsMatchId)))
            {
                Logger.Info($"Updating roster {roster.Id}");
                MatchRound matchRound = dict[roster.BitsMatchId];
                roster.OilPattern = OilPatternInformation.Create(
                    matchRound.MatchOilPatternName!,
                    matchRound.MatchOilPatternId);
                roster.Date = matchRound.MatchDate!.ToDateTime(matchRound.MatchTime);
                roster.Turn = matchRound.MatchRoundId;
                roster.MatchTimeChanged = matchRound.MatchStatus == 2;
                if (matchRound.HomeTeamClubId == websiteConfig.ClubId)
                {
                    roster.Team = matchRound.MatchHomeTeamAlias!;
                    roster.TeamLevel = roster.Team.Substring(roster.Team.LastIndexOf(' ') + 1);
                    roster.Opponent = matchRound.MatchAwayTeamAlias!;
                }
                else if (matchRound.AwayTeamClubId == websiteConfig.ClubId)
                {
                    roster.Team = matchRound.MatchAwayTeamAlias!;
                    roster.TeamLevel = roster.Team.Substring(roster.Team.LastIndexOf(' ') + 1);
                    roster.Opponent = matchRound.MatchHomeTeamAlias!;
                }
                else
                {
                    throw new Exception($"Unknown clubs: {matchRound.HomeTeamClubId} {matchRound.AwayTeamClubId}");
                }

                roster.Location = matchRound.MatchHallName!;
            }

            // add missing rosters
            HashSet<int> existingMatchIds = new(rosters.Select(x => x.BitsMatchId));
            foreach (int matchId in dict.Keys.Where(x => existingMatchIds.Contains(x) == false))
            {
                Logger.InfoFormat("Adding match {matchId}", matchId);
                MatchRound matchRound = dict[matchId];
                string team;
                string opponent;
                if (matchRound.HomeTeamClubId == websiteConfig.ClubId)
                {
                    team = matchRound.MatchHomeTeamAlias!;
                    opponent = matchRound.MatchAwayTeamAlias!;
                }
                else if (matchRound.AwayTeamClubId == websiteConfig.ClubId)
                {
                    team = matchRound.MatchAwayTeamAlias!;
                    opponent = matchRound.MatchHomeTeamAlias!;
                }
                else
                {
                    throw new Exception($"Unknown clubs: {matchRound.HomeTeamClubId} {matchRound.AwayTeamClubId}");
                }

                Roster roster = new(
                    matchRound.MatchSeason,
                    matchRound.MatchRoundId,
                    matchRound.MatchId,
                    team!,
                    team.Substring(team.LastIndexOf(' ') + 1),
                    matchRound.MatchHallName!,
                    opponent,
                    matchRound.MatchDate!.ToDateTime(matchRound.MatchTime),
                    matchRound.MatchNbrOfPlayers == 4,
                    OilPatternInformation.Create(matchRound.MatchOilPatternName!, matchRound.MatchOilPatternId))
                {
                    MatchTimeChanged = matchRound.MatchStatus == 2
                };
                CompositionRoot.DocumentSession.Store(roster);
            }
        }

        // remove extraneous rosters
        Roster[] toRemove = rosters.Where(x => foundMatchIds.Contains(x.BitsMatchId) == false).ToArray();
        if (toRemove.Any())
        {
            List<Roster> actualRemoved = new();
            foreach (Roster roster in toRemove)
            {
                // only regular season games, playoff is handled differently
                if (roster.MatchResultId is null && roster.Turn <= 20)
                {
                    CompositionRoot.DocumentSession.Delete(roster);
                    actualRemoved.Add(roster);
                }
            }

            string joined = string.Join(
                ",",
                actualRemoved.Select(x => $"Id={x.Id} BitsMatchId={x.BitsMatchId}"));
            Logger.InfoFormat("Rosters to remove: {joined}", joined);
            string body = $"Rosters to remove: {joined}";
            SendEmail email = SendEmail.ToAdmin(
                $"Removed rosters for {CompositionRoot.CurrentTenant.TeamFullName}",
                body);
            await CompositionRoot.EmailService.SendAsync(email);
        }
    }

    public record Command();
}
