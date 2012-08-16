﻿namespace Snittlistan.Controllers
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Infrastructure.Indexes;
    using Raven.Client;
    using Raven.Client.Linq;
    using ViewModels;

    /// <summary>
    /// Manages the start page.
    /// </summary>
    public class HomeController : AbstractController
    {
        /// <summary>
        /// Initializes a new instance of the HomeController class.
        /// </summary>
        /// <param name="session">Document session.</param>
        public HomeController(IDocumentSession session)
            : base(session)
        { }

        /// <summary>
        /// GET: /Home/Index.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var stats = Session.Query<Matches_PlayerStats.Result, Matches_PlayerStats>()
                .ToList()
                .OrderByDescending(s => s.AveragePins);

            var last20 = Session.Query<Pins_Last20.Result, Pins_Last20>()
                .ToDictionary(s => s.Player, s => s.Pins);

            var vm = stats.Select(s => new PlayerStatsViewModel(s, last20)).ToList();

            return View(vm);
        }

        /// <summary>
        /// GET: /Home/Player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public ViewResult Player(string player)
        {
            if (string.IsNullOrWhiteSpace(player))
                throw new HttpException(404, "Player not found");

            var q = Session.Query<Player_ByMatch.Result, Player_ByMatch>()
                .Where(r => r.Player == player)
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.BitsMatchId);

            var results = Session.Query<Matches_PlayerStats.Result, Matches_PlayerStats>()
                .SingleOrDefault(r => r.Player == player);

            var last20 = Session.Query<Pins_Last20.Result, Pins_Last20>()
                .SingleOrDefault(r => r.Player == player);

            return View(new PlayerMatchesViewModel
            {
                Player = player,
                Stats = q.ToList(),
                Results = results ?? new Matches_PlayerStats.Result(),
                Last20 = last20 ?? new Pins_Last20.Result()
            });
        }

        /// <summary>
        /// GET: /Home/About.
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        public ActionResult V2()
        {
            var turns = new TurnsViewModel[]
                {
                    new TurnsViewModel
                        {
                            Turn = 2,
                            StartDate = "22 sep",
                            EndDate = "23 sep",
                            Rosters = new RosterViewModel[]
                                {
                                    new RosterViewModel
                                        {
                                            Team = "Fredrikshof A",
                                            Location = "Birka",
                                            Opponent = "Stockholm IFK",
                                            Date = "lördag den 22 september 2012, 10:00",
                                            DeclinedClass = "warning",
                                            DeclinedCount = 2,
                                            DeclinedNames = "Kjell Jansson, Christer Liedholm"
                                        },
                                    new RosterViewModel
                                        {
                                            Team = "Fredrikshof F",
                                            Location = "Bowl-O-Rama",
                                            Opponent = "AIK F",
                                            Date = "lördag den 22 september 2012, 11:40",
                                            DeclinedClass = "warning",
                                            DeclinedCount = 2,
                                            DeclinedNames = "Daniel Lidström, Daniel Solvander"
                                        },
                                    new RosterViewModel
                                        {
                                            Team = "Fredrikshof B",
                                            Location = "Bowl-O-Rama",
                                            Opponent = "Hellas B",
                                            Date = "söndag den 23 september 2012, 10:00",
                                            DeclinedClass = "success",
                                            DeclinedCount = 0,
                                            DeclinedNames = string.Empty
                                        }
                                }
                        }
                };

            var players = new PlayerViewModel[]
                {
                    new PlayerViewModel
                        {
                            Name = "Daniel Lidström"
                        }
                };
            return View(new InitialDataViewModel
                {
                    Turns = turns,
                    Players = players
                });
        }
    }
}