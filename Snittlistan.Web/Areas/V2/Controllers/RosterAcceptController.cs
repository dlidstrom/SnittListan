﻿namespace Snittlistan.Web.Areas.V2.Controllers
{
    using System.Web.Mvc;
    using Domain;
    using Web.Controllers;

    [Authorize]
    public class RosterAcceptController : AbstractController
    {
        [HttpPost]
        public ActionResult Accept(string rosterId, string playerId, int season, int turn)
        {
            Roster roster = DocumentSession.Load<Roster>(rosterId);
            var update = new Roster.Update(
                Roster.ChangeType.PlayerAccepted,
                User.Identity.Name)
            {
                PlayerAccepted = playerId
            };
            roster.UpdateWith(update);
            return RedirectToAction("View", "Roster", new { season, turn });
        }
    }
}
