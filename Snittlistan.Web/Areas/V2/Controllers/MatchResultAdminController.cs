﻿namespace Snittlistan.Web.Areas.V2.Controllers
{
    using Indexes;
    using Raven.Abstractions;
    using Snittlistan.Web.Areas.V2.Commands;
    using Snittlistan.Web.Areas.V2.Domain;
    using Snittlistan.Web.Areas.V2.Domain.Match;
    using Snittlistan.Web.Areas.V2.ReadModels;
    using Snittlistan.Web.Areas.V2.ViewModels;
    using Snittlistan.Web.Controllers;
    using Snittlistan.Web.Helpers;
    using Snittlistan.Web.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [Authorize]
    public class MatchResultAdminController : AbstractController
    {
        private readonly IBitsClient bitsClient;

        public MatchResultAdminController(IBitsClient bitsClient)
        {
            this.bitsClient = bitsClient;
        }

        public ActionResult Register(int? season)
        {
            if (season.HasValue == false)
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);

            ViewBag.rosterid = DocumentSession.CreateRosterSelectList(season.Value);
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public ActionResult Register_RosterSelected(string rosterId)
        {
            var roster = DocumentSession.Load<Roster>(rosterId);
            if (roster == null)
                throw new HttpException(404, "Roster not found");
            return RedirectToAction("RegisterMatchEditor", new { rosterId  });
        }

        public ActionResult RegisterMatchEditor(string rosterId)
        {
            var roster = DocumentSession.Load<Roster>(rosterId);
            if (roster == null)
                throw new HttpException(404, "Roster not found");
            if (roster.MatchResultId != null)
                throw new HttpException(500, "Roster already registered");

            var availablePlayers = DocumentSession.Query<Player, PlayerSearch>()
                                                  .OrderBy(x => x.Name)
                                                  .Where(p => p.PlayerStatus == Player.Status.Active)
                                                  .ToList();
            var playerListItems = availablePlayers.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).ToArray();
            if (roster.IsFourPlayer)
            {
                var viewModel = new RegisterMatch4ViewModel(
                    DocumentSession.LoadRosterViewModel(roster),
                    playerListItems,
                    RegisterMatch4ViewModel.PostModel.ForCreate());
                return View(
                    "RegisterMatch4Editor",
                    viewModel);
            }

            return View("RegisterMatchEditor", new RegisterMatchViewModel(playerListItems));
        }

        [HttpPost]
        [ActionName("RegisterMatchEditor")]
        public ActionResult RegisterMatchEditorStore(string rosterId, RegisterMatch4ViewModel viewModel)
        {
            var roster = DocumentSession.Load<Roster>(rosterId);
            if (roster == null)
                throw new HttpException(404, "Roster not found");
            if (ModelState.IsValid == false)
            {
                var availablePlayers = DocumentSession.Query<Player, PlayerSearch>()
                                                      .OrderBy(x => x.Name)
                                                      .Where(p => p.PlayerStatus == Player.Status.Active)
                                                      .ToList();
                var playerListItems = availablePlayers.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                }).ToArray();
                if (roster.IsFourPlayer)
                {
                    viewModel.RosterViewModel = DocumentSession.LoadRosterViewModel(roster);
                    viewModel.PlayerListItems = playerListItems;
                    return View(
                        "RegisterMatch4Editor",
                        viewModel);
                }
            }

            return RedirectToAction(
                "RegisterMatchEditor",
                new
                {
                    rosterId
                });
            //return RedirectToAction("Details", "MatchResult", new { id = ResultHeaderReadModel.IdFromBitsMatchId(roster.BitsMatchId) });
        }

        public ActionResult RegisterConfirmed(int? season, RegisterResult vm)
        {
            if (ModelState.IsValid == false) return Register(season);

            var roster = DocumentSession.Load<Roster>(vm.RosterId);
            if (roster == null)
                throw new HttpException(404, "Roster not found");

            var matchResult = new MatchResult(
                roster,
                vm.TeamScore.GetValueOrDefault(),
                vm.OpponentScore.GetValueOrDefault(),
                vm.BitsMatchId.GetValueOrDefault());
            EventStoreSession.Store(matchResult);

            return RedirectToAction(
                "RegisterSerie",
                new
                {
                    aggregateId = matchResult.Id,
                    vm.RosterId,
                    vm.BitsMatchId
                });
        }

        public ActionResult EditResult(int id)
        {
            var matchId = ResultHeaderReadModel.IdFromBitsMatchId(id);
            var matchResult = DocumentSession.Load<ResultHeaderReadModel>(matchId);
            if (matchResult == null) throw new HttpException(404, "Match result not found");

            ViewBag.rosterid = DocumentSession.CreateRosterSelectList(matchResult.Season, matchResult.RosterId);

            ViewBag.Title = "Redigera matchresultat";
            return View("Register", new RegisterResult(matchResult));
        }

        [HttpPost]
        public ActionResult EditResult(int id, RegisterResult registerResult)
        {
            if (registerResult == null) throw new ArgumentNullException(nameof(registerResult));
            if (!ModelState.IsValid) return EditResult(id);

            var matchResult = EventStoreSession.Load<MatchResult>(registerResult.AggregateId);
            if (matchResult == null) throw new HttpException(404, "Match result not found");

            matchResult.Update(
                DocumentSession.Load<Roster>(registerResult.RosterId),
                registerResult.TeamScore.GetValueOrDefault(),
                registerResult.OpponentScore.GetValueOrDefault(),
                registerResult.BitsMatchId.GetValueOrDefault());

            return RedirectToAction("Index", "MatchResult");
        }

        public ActionResult RegisterSerie(string aggregateId, string rosterId, int bitsMatchId)
        {
            var roster = DocumentSession.Include<Roster>(r => r.Players)
                                        .Load<Roster>(rosterId);
            if (roster == null) throw new HttpException(404, "Roster not found");
            var registerSerie = new RegisterSerie(
                new ResultSeriesReadModel.Serie(),
                roster.Players.Select(
                    x => new SelectListItem
                    {
                        Text = DocumentSession.Load<Player>(x).Name,
                        Value = x
                    })
                    .ToList());
            return View(registerSerie);
        }

        [HttpPost]
        public ActionResult RegisterSerie(
            string aggregateId,
            int bitsMatchId,
            ResultSeriesReadModel.Serie serie)
        {
            var matchResult = EventStoreSession.Load<MatchResult>(aggregateId);
            if (matchResult == null) throw new HttpException(404, "Match result not found");
            var tables = new List<MatchTable>();
            for (var i = 0; i < 4; i++)
            {
                var game1 = new MatchGame(
                    serie.Tables[i].Game1.Player,
                    serie.Tables[i].Game1.Pins,
                    serie.Tables[i].Game1.Strikes,
                    serie.Tables[i].Game1.Spares);
                var game2 = new MatchGame(
                    serie.Tables[i].Game2.Player,
                    serie.Tables[i].Game2.Pins,
                    serie.Tables[i].Game2.Strikes,
                    serie.Tables[i].Game2.Spares);
                tables.Add(new MatchTable(i + 1, game1, game2, serie.Tables[i].Score));
            }

            matchResult.RegisterSerie(tables.ToArray());
            return RedirectToAction(
                "Details",
                "MatchResult",
                new
                {
                    id = bitsMatchId
                });
        }

        public ActionResult RegisterBits(int? season)
        {
            if (season.HasValue == false)
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);

            ViewBag.rosterid = DocumentSession.CreateBitsRosterSelectList(season.Value);
            return View(new RegisterBitsVerifyModel { Season = season.Value });
        }

        [HttpPost]
        public ActionResult RegisterBits(RegisterBitsVerifyModel model)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.rosterid = DocumentSession.CreateBitsRosterSelectList(model.Season);
                return View("RegisterBits", model);
            }

            var roster = DocumentSession.Include<Roster>(r => r.Players)
                                        .Load<Roster>(model.RosterId);
            if (roster == null)
                throw new HttpException(404, "Roster not found");

            var players = roster.Players
                                .Select(x => DocumentSession.Load<Player>(x))
                                .ToArray();
            var parser = new BitsParser(players);
            var content = bitsClient.DownloadMatchResult(roster.BitsMatchId);
            if (roster.IsFourPlayer)
            {
                var parse4Result = parser.Parse4(content, roster.Team);
                ExecuteCommand(new RegisterMatch4Command(roster, parse4Result));
            }
            else
            {
                var parseResult = parser.Parse(content, roster.Team);
                ExecuteCommand(new RegisterMatchCommand(roster, parseResult));
            }

            return RedirectToAction("Index", "MatchResult");
        }

        public class RegisterMatch4ViewModel
        {
            public RegisterMatch4ViewModel()
            {
            }

            public RegisterMatch4ViewModel(
                RosterViewModel rosterViewModel,
                SelectListItem[] playerListItems,
                PostModel postModel)
            {
                RosterViewModel = rosterViewModel;
                PlayerListItems = playerListItems;
                Model = postModel;
            }

            public RosterViewModel RosterViewModel { get; set; }

            public SelectListItem[] PlayerListItems { get; set; }

            public PostModel Model { get; set; }

            public class PostModel : IValidatableObject
            {
                public PostModel()
                {
                }

                public PostModel(PlayerGames[] players)
                {
                    Players = players;
                }

                [Required]
                [Range(0, 20)]
                [Display(Name = "Lagpoäng")]
                public int? TeamScore { get; set; }

                [Required]
                [Range(0, 20)]
                [Display(Name = "Motståndarpoäng")]
                public int? OpponentScore { get; set; }

                public PlayerGames[] Players { get; set; }

                public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
                {
                    if (TeamScore.GetValueOrDefault() + OpponentScore.GetValueOrDefault() > 20)
                    {
                        yield return new ValidationResult("Summan av lagpoängen kan inte överstiga 20.");
                    }
                    for (var i = 0; i < 4; i++)
                    {
                        if (Players.Count(x => x.Games[i].Pins.HasValue) != 4)
                        {
                            yield return new ValidationResult($"Ange 4 resultat i serie {i + 1}");
                        }
                    }
                }

                public static PostModel ForCreate()
                {
                    return new PostModel(
                        new[]
                        {
                            new PlayerGames(new PlayerGame[4]),
                            new PlayerGames(new PlayerGame[4]),
                            new PlayerGames(new PlayerGame[4]),
                            new PlayerGames(new PlayerGame[4]),
                            new PlayerGames(new PlayerGame[4])
                        });
                }
            }
        }

        public class RegisterMatchViewModel
        {
            public RegisterMatchViewModel(SelectListItem[] playerListItems)
            {
                Series = new RegisterSerie[4];
                PlayerListItems = playerListItems;
            }

            public RegisterMatchViewModel(
                int teamScore,
                int opponentScore,
                RegisterSerie[] series,
                SelectListItem[] playerListItems)
            {
                TeamScore = teamScore;
                OpponentScore = opponentScore;
                Series = series;
                PlayerListItems = playerListItems;
            }

            [Range(0, 20)]
            public int TeamScore { get; }

            [Range(0, 20)]
            public int OpponentScore { get; }

            public RegisterSerie[] Series { get; }

            public SelectListItem[] PlayerListItems { get; }
        }

        public class PlayerGame
        {
            public bool Score { get; set; }

            [Range(0, 300)]
            public int? Pins { get; set; }
        }

        public class PlayerGames
        {
            public PlayerGames()
            {
            }

            public PlayerGames(PlayerGame[] games)
            {
                Games = games;
            }

            public string PlayerId { get; set; }

            public PlayerGame[] Games { get; set; }
        }

        public class Register4Result
        {
            public Register4Result()
            {
                Series = new RegisterSerie4[4];
            }

            public Register4Result(ResultHeaderReadModel matchResult)
            {
                AggregateId = matchResult.AggregateId;
                TeamScore = matchResult.TeamScore;
                OpponentScore = matchResult.OpponentScore;
            }

            [HiddenInput]
            public string AggregateId { get; set; }

            [Required]
            public string RosterId { get; set; }

            [Range(0, 20), Required]
            public int? TeamScore { get; set; }

            [Range(0, 20), Required]
            public int? OpponentScore { get; set; }

            [Required]
            public int? BitsMatchId { get; set; }

            public RegisterSerie4[] Series { get; set; }
        }

        public class RegisterResult
        {
            public RegisterResult()
            {
                Series = new RegisterSerie[4];
            }

            public RegisterResult(ResultHeaderReadModel matchResult)
            {
                AggregateId = matchResult.AggregateId;
                TeamScore = matchResult.TeamScore;
                OpponentScore = matchResult.OpponentScore;
            }

            [HiddenInput]
            public string AggregateId { get; set; }

            [Required]
            public string RosterId { get; set; }

            [Range(0, 20), Required]
            public int? TeamScore { get; set; }

            [Range(0, 20), Required]
            public int? OpponentScore { get; set; }

            [Required]
            public int? BitsMatchId { get; set; }

            public RegisterSerie[] Series { get; set; }
        }
    }
}