﻿using System;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using Snittlistan.Controllers;
using Snittlistan.Models;
using Snittlistan.ViewModels;
using Xunit;

namespace Snittlistan.Test
{
	public class MatchController_EditDetails : DbTest
	{
		[Fact]
		public void CanEditDetails()
		{
			// Arrange
			var then = DateTime.Now.AddDays(-1);
			Match originalMatch = new Match("Place", then, 1, new Team("Home", 13), new Team("Away", 6));
			Session.Store(originalMatch);
			Session.SaveChanges();
			WaitForNonStaleResults<Match>();

			// Act
			var controller = new MatchController(Session);
			var now = DateTime.Now;
			var result = controller.EditDetails(new MatchViewModel.MatchDetails
			{
				Id = originalMatch.Id,
				Location = "NewPlace",
				Date = now,
				BitsMatchId = 2
			});

			// Assert
			result.AssertActionRedirect().ToAction("Details").WithParameter("id", originalMatch.Id);
			var match = Session.Load<Match>(originalMatch.Id);
			match.Location.ShouldBe("NewPlace");
			match.Date.ShouldBe(now);
			match.BitsMatchId.ShouldBe(2);
		}

		[Fact]
		public void CannotEditNonExistingMatch()
		{
			var controller = new MatchController(Session);
			var result = controller.EditDetails(1);

			// Assert
			result.AssertResultIs<HttpNotFoundResult>();
		}

		[Fact]
		public void CanEditTeam()
		{
			Assert.True(false, "Not implemented yet");
		}
	}
}
