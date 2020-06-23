﻿namespace Snittlistan.Web.Areas.V2.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Elmah;
    using Infrastructure;
    using Infrastructure.Bits;
    using Infrastructure.Bits.Contracts;
    using Newtonsoft.Json;
    using NLog;
    using Raven.Abstractions;
    using Raven.Client;
    using Snittlistan.Queue.Messages;
    using Snittlistan.Web.Areas.V2.Commands;
    using Snittlistan.Web.Areas.V2.Domain;
    using Snittlistan.Web.Areas.V2.Domain.Match;
    using Snittlistan.Web.Areas.V2.Indexes;
    using Snittlistan.Web.Areas.V2.Queries;
    using Snittlistan.Web.Areas.V2.ReadModels;
    using Snittlistan.Web.Controllers;
    using Snittlistan.Web.Helpers;
    using Snittlistan.Web.Infrastructure.Attributes;
    using Snittlistan.Web.Infrastructure.Indexes;
    using Snittlistan.Web.Models;

    [OnlyLocalAllowed]
    public class TaskController : AbstractApiController
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IBitsClient bitsClient;

        public TaskController(IBitsClient bitsClient)
        {
            this.bitsClient = bitsClient;
        }

        public async Task<IHttpActionResult> Post(TaskRequest request)
        {
            Log.Info($"Received task {request.TaskJson}");
            IHttpActionResult result = await HandleTask(request);
            Log.Info("Done");
            return result;
        }

        private async Task<IHttpActionResult> HandleTask(TaskRequest request)
        {
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            object taskObject = JsonConvert.DeserializeObject(
                request.TaskJson,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
            switch (taskObject)
            {
                case OneTimeKeyEvent @event:
                    return Handle(@event);
                case VerifyMatchMessage message:
                    return await Handle(message);
                case RegisterMatchesMessage message:
                    return Handle(message);
                case RegisterMatchMessage message:
                    return await Handle(message);
                case InitializeIndexesMessage message:
                    return Handle(message);
                case VerifyMatchesMessage message:
                    return Handle(message);
                case NewUserCreatedEvent @event:
                    return Handle(@event);
                case UserInvitedEvent @event:
                    return Handle(@event);
                case EmailTask task:
                    return Handle(task);
                case MatchRegisteredEvent @event:
                    return Handle(@event);
                case GetRostersFromBitsMessage message:
                    return await Handle(message);
                case GetPlayersFromBitsMessage message:
                    return await Handle(message);
            }

            Log.Error($"Unhandled task {taskObject.GetType()}");
            return Ok();
        }

        private async Task<IHttpActionResult> Handle(GetPlayersFromBitsMessage message)
        {
            WebsiteConfig websiteConfig = DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
            PlayerResult playersResult = await bitsClient.GetPlayers(websiteConfig.ClubId);
            Player[] players = DocumentSession.Query<Player, PlayerSearch>()
                .ToArray();

            // update existing players by matching on license number
            var playersByLicense = playersResult.Data.ToDictionary(x => x.LicNbr);
            foreach (Player player in players.Where(x => x.PlayerItem != null))
            {
                if (playersByLicense.TryGetValue(player.PlayerItem.LicNbr, out PlayerItem playerItem))
                {
                    player.PlayerItem = playerItem;
                    playersByLicense.Remove(player.PlayerItem.LicNbr);
                    Log.Info("Updating player with existing PlayerItem: {0}", player.PlayerItem.LicNbr);
                }
                else
                {
                    Log.Info("Player with {0} not found from BITS", player.PlayerItem.LicNbr);
                }
            }

            // add missing players, i.e. what is left from first step
            // try first to match on name, update those, add the rest
            var playerNamesWithoutPlayerItem = players.Where(x => x.PlayerItem == null).ToDictionary(x => x.Name);
            foreach (PlayerItem playerItem in playersByLicense.Values)
            {
                // look for name
                string nameFromBits = $"{playerItem.FirstName} {playerItem.SurName}";
                if (playerNamesWithoutPlayerItem.TryGetValue(nameFromBits, out Player player))
                {
                    player.PlayerItem = playerItem;
                    Log.Info("Updating player with missing PlayerItem: {0}", nameFromBits);
                }
                else
                {
                    // create new
                    var newPlayer = new Player(
                        $"{playerItem.FirstName} {playerItem.SurName}",
                        playerItem.Email,
                        playerItem.Inactive ? Player.Status.Inactive : Player.Status.Active,
                        playerItem.GetPersonalNumber(),
                        string.Empty,
                        new string[0])
                    {
                        PlayerItem = playerItem
                    };
                    Log.Info("Created player {0} {1}", playerItem.FirstName, playerItem.SurName);
                    DocumentSession.Store(newPlayer);
                }
            }

            return Ok();
        }

        private async Task<IHttpActionResult> Handle(GetRostersFromBitsMessage message)
        {
            WebsiteConfig websiteConfig = DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
            RosterSearchTerms.Result[] rosterSearchTerms =
                DocumentSession.Query<RosterSearchTerms.Result, RosterSearchTerms>()
                    .Where(x => x.Season == websiteConfig.SeasonId)
                    .Where(x => x.BitsMatchId != 0)
                    .ProjectFromIndexFieldsInto<RosterSearchTerms.Result>()
                    .ToArray();
            Roster[] rosters = DocumentSession.Load<Roster>(rosterSearchTerms.Select(x => x.Id));

            // Team
            TeamResult[] teams = await bitsClient.GetTeam(websiteConfig.ClubId, websiteConfig.SeasonId);
            foreach (TeamResult teamResult in teams)
            {
                // Division
                DivisionResult[] divisionResults = await bitsClient.GetDivisions(teamResult.TeamId, websiteConfig.SeasonId);

                // Match
                if (divisionResults.Length != 1) throw new Exception($"Unexpected number of divisions: {divisionResults.Length}");
                DivisionResult divisionResult = divisionResults[0];
                MatchRound[] matchRounds = await bitsClient.GetMatchRounds(teamResult.TeamId, divisionResult.DivisionId, websiteConfig.SeasonId);
                var dict = matchRounds.ToDictionary(x => x.MatchId);

                // update existing rosters
                foreach (Roster roster in rosters.Where(x => dict.ContainsKey(x.BitsMatchId)))
                {
                    MatchRound matchRound = dict[roster.BitsMatchId];
                    roster.OilPattern = OilPatternInformation.Create(
                        matchRound.MatchOilPatternName,
                        matchRound.MatchOilPatternId);
                    roster.Date = matchRound.MatchDate.ToDateTime(matchRound.MatchTime);
                    if (matchRound.HomeTeamClubId == websiteConfig.ClubId)
                    {
                        roster.Team = matchRound.MatchHomeTeamAlias;
                        roster.TeamLevel = roster.Team.Substring(roster.Team.LastIndexOf(' ') + 1);
                        roster.Opponent = matchRound.MatchAwayTeamAlias;
                    }
                    else if (matchRound.AwayTeamClubId == websiteConfig.ClubId)
                    {
                        roster.Team = matchRound.MatchAwayTeamAlias;
                        roster.TeamLevel = roster.Team.Substring(roster.Team.LastIndexOf(' ') + 1);
                        roster.Opponent = matchRound.MatchHomeTeamAlias;
                    }
                    else
                    {
                        throw new Exception($"Unknown clubs: {matchRound.HomeTeamClubId} {matchRound.AwayTeamClubId}");
                    }

                    roster.Location = matchRound.MatchHallName;
                }

                // add missing rosters
                var existingMatchIds = new HashSet<int>(rosters.Select(x => x.BitsMatchId));
                foreach (int matchId in dict.Keys.Where(x => existingMatchIds.Contains(x) == false))
                {
                    MatchRound matchRound = dict[matchId];
                    string team;
                    string opponent;
                    if (matchRound.HomeTeamClubId == websiteConfig.ClubId)
                    {
                        team = matchRound.MatchHomeTeamAlias;
                        opponent = matchRound.MatchAwayTeamAlias;
                    }
                    else if (matchRound.AwayTeamClubId == websiteConfig.ClubId)
                    {
                        team = matchRound.MatchAwayTeamAlias;
                        opponent = matchRound.MatchHomeTeamAlias;
                    }
                    else
                    {
                        throw new Exception($"Unknown clubs: {matchRound.HomeTeamClubId} {matchRound.AwayTeamClubId}");
                    }

                    var roster = new Roster(
                        matchRound.MatchSeason,
                        matchRound.MatchRoundId,
                        matchRound.MatchId,
                        team,
                        team.Substring(team.LastIndexOf(' ') + 1),
                        matchRound.MatchHallName,
                        opponent,
                        matchRound.MatchDate.ToDateTime(matchRound.MatchTime),
                        matchRound.MatchNbrOfPlayers == 4,
                        OilPatternInformation.Create(matchRound.MatchOilPatternName, matchRound.MatchOilPatternId));
                    DocumentSession.Store(roster);
                }
            }

            return Ok();
        }

        private IHttpActionResult Handle(OneTimeKeyEvent @event)
        {
            const string Subject = "Logga in på Snittlistan";
            Emails.SendOneTimePassword(@event.Email, Subject, @event.OneTimePassword);
            return Ok();
        }

        private async Task<IHttpActionResult> Handle(VerifyMatchMessage message)
        {
            Roster roster = DocumentSession.Load<Roster>(message.RosterId);
            if (roster.IsVerified) return Ok();
            WebsiteConfig websiteConfig = DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
            HeadInfo result = await bitsClient.GetHeadInfo(roster.BitsMatchId);
            ParseHeaderResult header = BitsParser.ParseHeader(result, websiteConfig.ClubId);

            // chance to update roster values
            roster.OilPattern = header.OilPattern;
            roster.Date = header.Date;
            roster.Opponent = header.Opponent;
            roster.Location = header.Location;
            if (roster.MatchResultId == null) return Ok();

            // update match result values
            BitsMatchResult bitsMatchResult = await bitsClient.GetBitsMatchResult(roster.BitsMatchId);
            Player[] players = DocumentSession.Query<Player, PlayerSearch>()
                .ToArray()
                .Where(x => x.PlayerItem?.LicNbr != null)
                .ToArray();
            var parser = new BitsParser(players);
            if (roster.IsFourPlayer)
            {
                MatchResult4 matchResult = EventStoreSession.Load<MatchResult4>(roster.MatchResultId);
                Parse4Result parseResult = parser.Parse4(bitsMatchResult, websiteConfig.ClubId);
                roster.Players = GetPlayerIds(parseResult);
                bool isVerified = matchResult.Update(
                    PublishMessage,
                    roster,
                    parseResult.TeamScore,
                    parseResult.OpponentScore,
                    roster.BitsMatchId,
                    parseResult.CreateMatchSeries(),
                    players);
                roster.IsVerified = isVerified;
            }
            else
            {
                MatchResult matchResult = EventStoreSession.Load<MatchResult>(roster.MatchResultId);
                ParseResult parseResult = parser.Parse(bitsMatchResult, websiteConfig.ClubId);
                roster.Players = GetPlayerIds(parseResult);
                var resultsForPlayer = DocumentSession.Query<ResultForPlayerIndex.Result, ResultForPlayerIndex>()
                    .Where(x => x.Season == roster.Season)
                    .ToArray()
                    .ToDictionary(x => x.PlayerId);
                MatchSerie[] matchSeries = parseResult.CreateMatchSeries();
                bool isVerified = matchResult.Update(
                    PublishMessage,
                    roster,
                    parseResult.TeamScore,
                    parseResult.OpponentScore,
                    matchSeries,
                    parseResult.OpponentSeries,
                    players,
                    resultsForPlayer);
                roster.IsVerified = isVerified;
            }

            return Ok();
        }

        private async Task<IHttpActionResult> Handle(RegisterMatchMessage message)
        {
            WebsiteConfig websiteConfig = DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
            Player[] players =
                DocumentSession.Query<Player, PlayerSearch>()
                    .ToArray()
                    .Where(x => x.PlayerItem?.LicNbr != null)
                    .ToArray();
            Roster pendingMatch = DocumentSession.Load<Roster>(message.RosterId);
            try
            {
                var parser = new BitsParser(players);
                BitsMatchResult bitsMatchResult = await bitsClient.GetBitsMatchResult(pendingMatch.BitsMatchId);
                if (pendingMatch.IsFourPlayer)
                {
                    Parse4Result parse4Result = parser.Parse4(bitsMatchResult, websiteConfig.ClubId);
                    if (parse4Result != null)
                    {
                        List<string> allPlayerIds = GetPlayerIds(parse4Result);
                        pendingMatch.Players = allPlayerIds;
                        ExecuteCommand(new RegisterMatch4Command(pendingMatch, parse4Result));
                    }
                }
                else
                {
                    ParseResult parseResult = parser.Parse(bitsMatchResult, websiteConfig.ClubId);
                    if (parseResult != null)
                    {
                        List<string> allPlayerIds = GetPlayerIds(parseResult);
                        pendingMatch.Players = allPlayerIds;
                        ExecuteCommand(new RegisterMatchCommand(pendingMatch, parseResult));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorSignal
                    .FromCurrentContext()
                    .Raise(new Exception($"Unable to auto register match {pendingMatch.Id} ({pendingMatch.BitsMatchId})", e));
                return Ok(e.ToString());
            }

            return Ok();
        }

        private IHttpActionResult Handle(RegisterMatchesMessage message)
        {
            WebsiteConfig websiteConfig = DocumentSession.Load<WebsiteConfig>(WebsiteConfig.GlobalId);
            Roster[] pendingMatches = ExecuteQuery(new GetPendingMatchesQuery(websiteConfig.SeasonId));
            foreach (Roster pendingMatch in pendingMatches)
            {
                PublishMessage(new RegisterMatchMessage(pendingMatch.Id));
            }

            return Ok();
        }

        private static List<string> GetPlayerIds(Parse4Result parse4Result)
        {
            IEnumerable<string> query = from game in parse4Result.Series.First().Games
                                        select game.Player;
            string[] playerIds = query.ToArray();
            var playerIdsWithoutReserve = new HashSet<string>(playerIds);
            IEnumerable<string> restQuery = from serie in parse4Result.Series
                                            from game in serie.Games
                                            where playerIdsWithoutReserve.Contains(game.Player) == false
                                            select game.Player;
            var allPlayerIds = playerIds.Concat(
                new HashSet<string>(restQuery).Where(x => playerIdsWithoutReserve.Contains(x) == false)).ToList();
            return allPlayerIds;
        }

        private static List<string> GetPlayerIds(ParseResult parseResult)
        {
            IEnumerable<string> query = from table in parseResult.Series.First().Tables
                                        from game in new[] { table.Game1, table.Game2 }
                                        select game.Player;
            string[] playerIds = query.ToArray();
            var playerIdsWithoutReserve = new HashSet<string>(playerIds);
            IEnumerable<string> restQuery = from serie in parseResult.Series
                                            from table in serie.Tables
                                            from game in new[] { table.Game1, table.Game2 }
                                            where playerIdsWithoutReserve.Contains(game.Player) == false
                                            select game.Player;
            var allPlayerIds = playerIds.Concat(
                new HashSet<string>(restQuery).Where(x => playerIdsWithoutReserve.Contains(x) == false)).ToList();
            return allPlayerIds;
        }

        private IHttpActionResult Handle(InitializeIndexesMessage message)
        {
            IndexCreator.CreateIndexes(DocumentStore);
            User admin = DocumentSession.Load<User>(Models.User.AdminId);
            if (admin == null)
            {
                admin = new User("", "", message.Email, message.Password)
                {
                    Id = Models.User.AdminId
                };
                admin.Initialize(PublishMessage);
                admin.Activate();
                DocumentSession.Store(admin);
            }
            else
            {
                admin.SetEmail(message.Email);
                admin.SetPassword(message.Password);
                admin.Activate();
            }

            return Ok();
        }

        private IHttpActionResult Handle(VerifyMatchesMessage message)
        {
            int season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);
            Roster[] rosters = DocumentSession.Query<Roster, RosterSearchTerms>()
                                         .Where(x => x.Season == season)
                                         .ToArray();
            var toVerify = new List<VerifyMatchMessage>();
            foreach (Roster roster in rosters)
            {
                if (roster.BitsMatchId == 0)
                {
                    continue;
                }

                if (roster.IsVerified && message.Force == false)
                {
                    Log.Info($"Skipping {roster.BitsMatchId} because it is already verified.");
                }
                else
                {
                    toVerify.Add(new VerifyMatchMessage(roster.BitsMatchId, roster.Id));
                }
            }

            foreach (VerifyMatchMessage verifyMatchMessage in toVerify)
            {
                PublishMessage(verifyMatchMessage);
            }

            return Ok();
        }

        private IHttpActionResult Handle(NewUserCreatedEvent @event)
        {
            string recipient = @event.Email;
            const string Subject = "Välkommen till Snittlistan!";
            string activationKey = @event.ActivationKey;
            string id = @event.UserId;

            Emails.UserRegistered(recipient, Subject, id, activationKey);

            return Ok();
        }

        private IHttpActionResult Handle(UserInvitedEvent @event)
        {
            string recipient = @event.Email;
            const string Subject = "Välkommen till Snittlistan!";
            string activationUri = @event.ActivationUri;

            Emails.InviteUser(recipient, Subject, activationUri);

            return Ok();
        }

        private IHttpActionResult Handle(EmailTask task)
        {
            Emails.SendMail(
                task.Recipient,
                Encoding.UTF8.GetString(Convert.FromBase64String(task.Subject)),
                Encoding.UTF8.GetString(Convert.FromBase64String(task.Content)));

            return Ok();
        }

        private IHttpActionResult Handle(MatchRegisteredEvent @event)
        {
            Roster roster = DocumentSession.Load<Roster>(@event.RosterId);
            if (roster.IsFourPlayer) return Ok();
            string resultSeriesReadModelId = ResultSeriesReadModel.IdFromBitsMatchId(roster.BitsMatchId, roster.Id);
            ResultSeriesReadModel resultSeriesReadModel = DocumentSession.Load<ResultSeriesReadModel>(resultSeriesReadModelId);
            string resultHeaderReadModelId = ResultHeaderReadModel.IdFromBitsMatchId(roster.BitsMatchId, roster.Id);
            ResultHeaderReadModel resultHeaderReadModel = DocumentSession.Load<ResultHeaderReadModel>(resultHeaderReadModelId);
            Emails.MatchRegistered(
                roster.Team,
                roster.Opponent,
                @event.Score,
                @event.OpponentScore,
                resultSeriesReadModel,
                resultHeaderReadModel);

            return Ok();
        }

        public class TaskRequest
        {
            public TaskRequest(string taskJson)
            {
                TaskJson = taskJson;
            }

            [Required]
            public string TaskJson { get; }
        }
    }
}
