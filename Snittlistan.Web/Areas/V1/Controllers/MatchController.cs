﻿using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Linq;
using Snittlistan.Web.Areas.V1.Models;
using Snittlistan.Web.Areas.V1.ViewModels.Match;
using Snittlistan.Web.Controllers;
using Snittlistan.Web.Helpers;
using Snittlistan.Web.Infrastructure.AutoMapper;
using Snittlistan.Web.Infrastructure.Indexes;

namespace Snittlistan.Web.Areas.V1.Controllers
{
    public class MatchController : AbstractController
    {
        /// <summary>
        /// GET: /Match/.
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            var matches = DocumentSession.Query<Match_ByDate.Result, Match_ByDate>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .OrderByDescending(m => m.Date)
                .ProjectFromIndexFieldsInto<Match_ByDate.Result>()
                .ToList();

            return View(matches);
        }

        /// <summary>
        /// GET: /Match/Details8x4/5.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details8x4(int id)
        {
            var match = DocumentSession.Load<Match8x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            var matchId = DocumentSession.Advanced.GetDocumentId(match);
            var results = DocumentSession.Query<Player_ByMatch.Result, Player_ByMatch>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .Where(x => x.MatchId == matchId)
                .OrderByDescending(x => x.Pins);

            var vm = new Match8x4ViewModel
            {
                Match = match.MapTo<Match8x4ViewModel.MatchDetails>(),
                HomeTeam = match.HomeTeam.MapTo<Team8x4DetailsViewModel>(),
                HomeTeamResults = results.Where(g => g.Team == match.HomeTeam.Name).ToList(),
                AwayTeam = match.AwayTeam.MapTo<Team8x4DetailsViewModel>(),
                AwayTeamResults = results.Where(g => g.Team == match.AwayTeam.Name).ToList(),
            };

            return View(vm);
        }

        /// <summary>
        /// GET: /Match/Register.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ViewResult Register8x4()
        {
            return View(new Register8x4MatchViewModel());
        }

        /// <summary>
        /// POST: /Match/Register.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult Register8x4(Register8x4MatchViewModel model)
        {
            if (DocumentSession.BitsIdExists(model.BitsMatchId))
                ModelState.AddModelError("BitsMatchId", "Matchen redan registrerad");

            if (!ModelState.IsValid)
                return View(model);

            var match = new Match8x4(
                model.Location,
                model.Date,
                model.BitsMatchId,
                model.HomeTeam.MapTo<HomeTeamFactory>().CreateTeam(),
                model.AwayTeam.MapTo<AwayTeamFactory>().CreateTeam());
            DocumentSession.Store(match);

            return RedirectToAction("Details8x4", new { id = match.Id });
        }

        /// <summary>
        /// GET: /Match/Edit/5.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditDetails8x4(int id)
        {
            var match = DocumentSession.Load<Match8x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            return View(match.MapTo<Match8x4ViewModel.MatchDetails>());
        }

        /// <summary>
        /// POST: /Match/Edit/5.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult EditDetails8x4(Match8x4ViewModel.MatchDetails model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var match = DocumentSession.Load<Match8x4>(model.Id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            match.Location = model.Location;
            match.Date = model.Date;
            match.BitsMatchId = model.BitsMatchId;

            return RedirectToAction("Details8x4", new { id = match.Id });
        }

        /// <summary>
        /// GET: /Match/EditTeam/5.
        /// </summary>
        /// <param name="id">Match identifier.</param>
        /// <param name="isHomeTeam">True if home team; false if away team.</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditTeam8x4(int id, bool isHomeTeam)
        {
            var match = DocumentSession.Load<Match8x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            var teamViewModel = isHomeTeam
                ? match.HomeTeam.MapTo<Team8x4ViewModel>()
                : match.AwayTeam.MapTo<Team8x4ViewModel>();
            var vm = new EditTeam8x4ViewModel
            {
                Id = id,
                IsHomeTeam = isHomeTeam,
                Team = teamViewModel
            };

            return View(vm);
        }

        /// <summary>
        /// POST: /Match/EditTeam/5.
        /// </summary>
        /// <param name="vm">Team view model.</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult EditTeam8x4(EditTeam8x4ViewModel vm)
        {
            var match = DocumentSession.Load<Match8x4>(vm.Id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            if (vm.IsHomeTeam)
                match.HomeTeam = vm.Team.MapTo<HomeTeamFactory>().CreateTeam();
            else
                match.AwayTeam = vm.Team.MapTo<AwayTeamFactory>().CreateTeam();

            return RedirectToAction("Details8x4", new { id = vm.Id });
        }

        [Authorize]
        public ActionResult Delete8x4(int id)
        {
            var match = DocumentSession.Load<Match8x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            return View(match.MapTo<Match8x4ViewModel.MatchDetails>());
        }

        [Authorize, HttpPost]
        public ActionResult Delete8x4(Match8x4ViewModel.MatchDetails vm)
        {
            var match = DocumentSession.Load<Match8x4>(vm.Id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            DocumentSession.Delete(match);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Match/Register.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ViewResult Register4x4()
        {
            return View(new Register4x4MatchViewModel());
        }

        [HttpPost, Authorize]
        public ActionResult Register4x4(Register4x4MatchViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var match = new Match4x4(
                model.Location,
                model.Date,
                model.HomeTeam.MapTo<Team4x4>(),
                model.AwayTeam.MapTo<Team4x4>());
            DocumentSession.Store(match);

            return RedirectToAction("Details4x4", new { id = match.Id });
        }

        /// <summary>
        /// GET: /Match/Edit/5.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditDetails4x4(int id)
        {
            var match = DocumentSession.Load<Match4x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            return View(match.MapTo<Match4x4ViewModel.MatchDetails>());
        }

        /// <summary>
        /// POST: /Match/Edit/5.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult EditDetails4x4(Match4x4ViewModel.MatchDetails model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var match = DocumentSession.Load<Match4x4>(model.Id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            match.Location = model.Location;
            match.Date = model.Date;

            return RedirectToAction("Details4x4", new { id = match.Id });
        }

        /// <summary>
        /// GET: /Match/EditTeam/5.
        /// </summary>
        /// <param name="id">Match identifier.</param>
        /// <param name="isHomeTeam">True if home team; false if away team.</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditTeam4x4(int id, bool isHomeTeam)
        {
            var match = DocumentSession.Load<Match4x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            var teamViewModel = isHomeTeam
                ? match.HomeTeam.MapTo<Team4x4ViewModel>()
                : match.AwayTeam.MapTo<Team4x4ViewModel>();
            var vm = new EditTeam4x4ViewModel
            {
                Id = id,
                IsHomeTeam = isHomeTeam,
                Team = teamViewModel
            };

            return View(vm);
        }

        /// <summary>
        /// POST: /Match/EditTeam/5.
        /// </summary>
        /// <param name="vm">Team view model.</param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult EditTeam4x4(EditTeam4x4ViewModel vm)
        {
            var match = DocumentSession.Load<Match4x4>(vm.Id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            if (vm.IsHomeTeam)
                match.HomeTeam = vm.Team.MapTo<Team4x4>();
            else
                match.AwayTeam = vm.Team.MapTo<Team4x4>();

            return RedirectToAction("Details4x4", new { id = vm.Id });
        }

        [Authorize]
        public ActionResult Delete4x4(int id)
        {
            var match = DocumentSession.Load<Match4x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            return View(match.MapTo<Match4x4ViewModel.MatchDetails>());
        }

        [Authorize, HttpPost]
        public ActionResult Delete4x4(Match4x4ViewModel.MatchDetails vm)
        {
            var match = DocumentSession.Load<Match4x4>(vm.Id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            DocumentSession.Delete(match);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /Match/Details4x4/5.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details4x4(int id)
        {
            var match = DocumentSession.Load<Match4x4>(id);
            if (match == null)
                throw new HttpException(404, "Match not found");

            var matchId = DocumentSession.Advanced.GetDocumentId(match);
            var results = DocumentSession.Query<Player_ByMatch.Result, Player_ByMatch>()
                .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                .Where(x => x.MatchId == matchId)
                .OrderByDescending(x => x.Pins);

            var vm = new Match4x4ViewModel
            {
                Match = match.MapTo<Match4x4ViewModel.MatchDetails>(),
                HomeTeam = match.HomeTeam.MapTo<Team4x4DetailsViewModel>(),
                HomeTeamResults = results.Where(r => r.Team == match.HomeTeam.Name).ToList(),
                AwayTeam = match.AwayTeam.MapTo<Team4x4DetailsViewModel>(),
                AwayTeamResults = results.Where(r => r.Team == match.AwayTeam.Name).ToList(),
            };

            return View(vm);
        }

        public ActionResult Create()
        {
            return RedirectToActionPermanent("Index");
        }

        public ActionResult Edit()
        {
            return RedirectToActionPermanent("Index");
        }

        public ActionResult LegacyRedirect()
        {
            return RedirectToActionPermanent("Index");
        }
    }
}