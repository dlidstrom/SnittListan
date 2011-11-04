﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Raven.Client;
using Snittlistan.Infrastructure.Indexes;

namespace Snittlistan.Controllers
{
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
				.OrderByDescending(s => s.Average)
				.ToList();

			return View(stats);
		}

		/// <summary>
		/// GET: /Home/About.
		/// </summary>
		/// <returns></returns>
		public ActionResult About()
		{
			return View();
		}
	}
}
