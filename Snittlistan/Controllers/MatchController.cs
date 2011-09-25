﻿using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using Snittlistan.Infrastructure;
using Snittlistan.Models;
using Snittlistan.ViewModels;

namespace Snittlistan.Controllers
{
	public class MatchController : AbstractController
	{
		/// <summary>
		/// Initializes a new instance of the MatchController class.
		/// </summary>
		/// <param name="session">Session instance.</param>
		public MatchController(IDocumentSession session)
			: base(session)
		{
		}

		/// <summary>
		/// GET: /Match/.
		/// </summary>
		/// <returns></returns>
		public ViewResult Index()
		{
			var matches = Session.Query<Match>()
				.ToList()
				.Select(m => new MatchViewModel
				{
					Match = m.MapTo<MatchViewModel.MatchDetails>(),
					HomeTeam = m.HomeTeam.MapTo<TeamViewModel>(),
					AwayTeam = m.AwayTeam.MapTo<TeamViewModel>()
				});
			return View(matches);
		}

		/// <summary>
		/// GET: /Match/Details/5.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ActionResult Details(int id)
		{
			var match = Session.Load<Match>(id);
			if (match == null)
				return HttpNotFound();

			var vm = new MatchViewModel
			{
				Match = match.MapTo<MatchViewModel.MatchDetails>(),
				HomeTeam = match.HomeTeam.MapTo<TeamViewModel>(),
				AwayTeam = match.AwayTeam.MapTo<TeamViewModel>()
			};

			return View(vm);
		}

		/// <summary>
		/// GET: /Match/Register.
		/// </summary>
		/// <returns></returns>
		public ViewResult Register()
		{
			return View(new RegisterMatchViewModel());
		}

		/// <summary>
		/// POST: /Match/Register.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Register(RegisterMatchViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var match = new Match(
				model.Location,
				model.Date,
				model.BitsMatchId,
				model.HomeTeam.MapTo<Team>(),
				model.AwayTeam.MapTo<Team>());
			Session.Store(match);

			return RedirectToAction("Details", new { id = match.Id });
		}

		/// <summary>
		/// GET: /Match/Edit/5.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ActionResult EditDetails(int id)
		{
			var match = Session.Load<Match>(id);
			if (match == null)
				return HttpNotFound();

			return View(match.MapTo<MatchViewModel.MatchDetails>());
		}

		/// <summary>
		/// POST: /Match/Edit/5.
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult EditDetails(MatchViewModel.MatchDetails model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var match = Session.Load<Match>(model.Id);
			if (match == null)
				return HttpNotFound();

			match.Location = model.Location;
			match.Date = model.Date;
			match.BitsMatchId = model.BitsMatchId;

			return RedirectToAction("Details", new { id = match.Id });
		}

		/// <summary>
		/// GET: /Match/EditTeam/5.
		/// </summary>
		/// <param name="id">Match identifier.</param>
		/// <param name="isHomeTeam">True if home team; false if away team.</param>
		/// <returns></returns>
		public ActionResult EditTeam(int id, bool isHomeTeam)
		{
			var match = Session.Load<Match>(id);
			if (match == null)
				return HttpNotFound();

			var vm = new EditTeamViewModel
			{
				Id = id,
				IsHomeTeam = isHomeTeam,
				Team = match.HomeTeam.MapTo<TeamViewModel>()
			};

			return View(vm);
		}

		/// <summary>
		/// POST: /Match/EditTeam/5.
		/// </summary>
		/// <param name="vm">Team view model.</param>
		/// <returns></returns>
		public ActionResult EditTeam(EditTeamViewModel vm)
		{
			var match = Session.Load<Match>(vm.Id);
			if (match == null)
				return HttpNotFound();

			if (vm.IsHomeTeam)
				match.HomeTeam = vm.Team.MapTo<Team>();
			else
				match.AwayTeam = vm.Team.MapTo<Team>();

			return RedirectToAction("Details", new { id = vm.Id });
		}

		/// <summary>
		/// GET: /Match/Delete/5.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ActionResult Delete(int id)
		{
			var match = Session.Load<Match>(id);
			if (match == null)
				return HttpNotFound();

			return View(match.MapTo<MatchViewModel.MatchDetails>());
		}

		/// <summary>
		/// POST: /Match/Delete/5.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(string id)
		{
			try
			{
				// TODO: Add delete logic here
				return RedirectToAction("Index");
			}
			catch
			{
				return View();
			}
		}
	}
}
