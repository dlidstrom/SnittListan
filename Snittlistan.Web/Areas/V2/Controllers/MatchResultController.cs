﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Abstractions;
using Snittlistan.Web.Areas.V2.Domain;
using Snittlistan.Web.Areas.V2.Indexes;
using Snittlistan.Web.Areas.V2.ReadModels;
using Snittlistan.Web.Areas.V2.ViewModels;
using Snittlistan.Web.Controllers;
using Snittlistan.Web.Helpers;

namespace Snittlistan.Web.Areas.V2.Controllers
{
    public class MatchResultController : AbstractController
    {
        public ActionResult Index(int? season)
        {
            if (season.HasValue == false)
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);

            var headerReadModels = DocumentSession.Query<ResultHeaderReadModel, ResultHeaderIndex>()
                                                  .Where(x => x.Season == season)
                                                  .ToList();
            var vm = new MatchResultViewModel
            {
                SeasonStart = season.Value,
                Turns = headerReadModels
                    .GroupBy(x => x.Turn)
                    .ToDictionary(x => x.Key, x => x.ToList())
            };
            return View(vm);
        }

        public ActionResult Details(int id)
        {
            var headerId = ResultHeaderReadModel.IdFromBitsMatchId(id);
            var headerReadModel = DocumentSession.Load<ResultHeaderReadModel>(headerId);
            if (headerReadModel == null) throw new HttpException(404, "Match result not found");

            if (headerReadModel.IsFourPlayer)
            {
                var matchId = ResultSeries4ReadModel.IdFromBitsMatchId(id);
                var resultReadModel = DocumentSession.Load<ResultSeries4ReadModel>(matchId)
                    ?? new ResultSeries4ReadModel();

                return View("Details4", new Result4ViewModel(headerReadModel, resultReadModel));
            }
            else
            {
                var matchId = ResultSeriesReadModel.IdFromBitsMatchId(id);
                var resultReadModel = DocumentSession.Load<ResultSeriesReadModel>(matchId)
                    ?? new ResultSeriesReadModel();

                return View(new ResultViewModel(headerReadModel, resultReadModel));
            }
        }

        public ActionResult Turns(int? season)
        {
            if (season.HasValue == false)
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);

            var weeks = DocumentSession.Query<TeamOfWeek, TeamOfWeekIndex>()
                .Where(x => x.Season == season.Value)
                .OrderByDescending(x => x.Turn)
                .ToArray();
            var viewModel = new TeamOfWeekViewModel(season.Value, weeks);
            return View(viewModel);
        }

        public ActionResult Form(int? season)
        {
            if (season.HasValue == false)
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);

            var players = DocumentSession.Query<Player, PlayerSearch>()
                .ToArray();

            var results = DocumentSession.Query<ResultForPlayerIndex.Result, ResultForPlayerIndex>()
                .Where(x => x.Season == season.Value)
                .ToArray();
            var seasonAverages = results.ToDictionary(x => x.PlayerId);

            var response = new List<PlayerFormViewModel>();
            foreach (var player in players)
            {
                var name = player.Name;
                ResultForPlayerIndex.Result result;
                if (seasonAverages.TryGetValue(player.Id, out result)
                    && result.TotalSeries > 0)
                {
                    var playerForm = new PlayerFormViewModel(name)
                    {
                        TotalSeries = result.TotalSeries,
                        TotalScore = result.TotalScore,
                        ScoreAverage = (double)result.TotalScore / Math.Max(1, result.TotalSeries),
                        SeasonAverage = (double)result.TotalPins / Math.Max(1, result.TotalSeries),
                        Last5Average = (double)result.Last5TotalPins / Math.Max(1, result.Last5TotalSeries),
                        HasResult = true
                    };
                    response.Add(playerForm);
                }
                else if (player.PlayerStatus == Player.Status.Active)
                {
                    response.Add(new PlayerFormViewModel(name));
                }
            }

            var viewModel = new FormViewModel(
                season.Value,
                response.OrderByDescending(x => x.SeasonAverage).ThenBy(x => x.Name).ToArray());
            return View(viewModel);
        }

        public ActionResult EliteMedals(int? season)
        {
            if (season.HasValue == false)
            {
                season = DocumentSession.LatestSeasonOrDefault(SystemTime.UtcNow.Year);
            }

            var playersDict = DocumentSession.Query<Player, PlayerSearch>()
                .Where(p => p.PlayerStatus == Player.Status.Active)
                .ToDictionary(x => x.Id);
            var eliteMedals = DocumentSession.Load<EliteMedals>(Domain.EliteMedals.TheId);
            if (eliteMedals == null)
            {
                eliteMedals = new EliteMedals();
                DocumentSession.Store(eliteMedals);
            }

            var seasonResults = DocumentSession.Load<SeasonResults>(SeasonResults.GetId(season.Value));
            if (seasonResults == null)
            {
                seasonResults = new SeasonResults(season.Value);
                DocumentSession.Store(seasonResults);
            }

            return View(new EliteMedalsViewModel(season.Value, playersDict, eliteMedals, seasonResults));
        }

        [Authorize]
        public ActionResult EditMedals(int id)
        {
            var player = DocumentSession.Load<Player>(id);
            var eliteMedals = DocumentSession.Load<EliteMedals>(Domain.EliteMedals.TheId);
            var eliteMedal = eliteMedals.GetExistingMedal(player.Id);
            var viewModel = new EditMedalsViewModel(
                player.Name,
                eliteMedal.Value,
                eliteMedal.CapturedSeason.GetValueOrDefault());
            return View(viewModel);
        }

        [HttpPost, Authorize, ActionName("EditMedals")]
        public ActionResult EditMedalsPost(int id, EditMedalsPostModel postModel)
        {
            if (ModelState.IsValid == false)
            {
                return EditMedals(id);
            }

            var eliteMedals = DocumentSession.Load<EliteMedals>(Domain.EliteMedals.TheId);
            Debug.Assert(postModel.EliteMedal != null, "postModel.EliteMedal != null");
            eliteMedals.AwardMedal("players-" + id, postModel.EliteMedal.Value, postModel.CapturedSeason);
            return RedirectToAction("EliteMedals");
        }
    }
}