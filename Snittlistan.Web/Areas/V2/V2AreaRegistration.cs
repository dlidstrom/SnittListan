﻿namespace Snittlistan.Web.Areas.V2
{
    using System.Web.Mvc;

    public class V2AreaRegistration : AreaRegistration
    {
        public override string AreaName => "V2";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Redirects1",
                url: "Home/Player/{*rest}",
                defaults: new { controller = "Redirect", action = "Redirect" });
            context.MapRoute(
                name: "Redirects2",
                url: "Match/Details4x4/{*rest}",
                defaults: new { controller = "Redirect", action = "Redirect" });
            context.MapRoute(
                name: "Redirects3",
                url: "Match/Details8x4/{*rest}",
                defaults: new { controller = "Redirect", action = "Redirect" });
            context.MapRoute(
                name: "Redirects4",
                url: "Match",
                defaults: new { controller = "Redirect", action = "Redirect" });
            context.MapRoute(
                name: "Redirects5",
                url: "register",
                defaults: new { controller = "Redirect", action = "Redirect" });
            context.MapRoute(
                name: "Redirects6",
                url: "Account/Register",
                defaults: new { controller = "Redirect", action = "Redirect" });
            context.MapRoute(
                name: "Redirects7",
                url: "Roster/{season}",
                defaults: new
                {
                    controller = "Redirect",
                    action = "RedirectNewView"
                },
                constraints: new
                {
                    season = @"\d+"
                });
            context.MapRoute(
                name: "Redirects8",
                url: "Roster/{season}/{turn}",
                defaults: new
                {
                    controller = "Redirect",
                    action = "RedirectNewView"
                },
                constraints: new
                {
                    season = @"\d+",
                    turn = @"\d+"
                });

            context.MapRoute(
                name: "SearchTerms-Route",
                url: "Search/{action}",
                defaults: new { controller = "SearchTerms" });

            // register match result
            context.MapRoute(
                "RegisterMatchResult",
                "MatchResult/Register/{season}",
                new { controller = "MatchResult", action = "Register" });

            RosterRoutes(context);

            // default route
            context.MapRoute(
                "V2_default",
                "{controller}/{action}/{id}",
                new { controller = "Roster", action = "Index", id = UrlParameter.Optional });
        }

        private static void RosterRoutes(AreaRegistrationContext context)
        {
            // create roster
            context.MapRoute(
                "CreateRoster",
                "Roster/Create",
                new
                {
                    controller = "Roster",
                    action = "Create"
                });

            // edit roster
            context.MapRoute(
                "EditRoster",
                "Roster/Edit/{id}",
                new
                {
                    controller = "Roster",
                    action = "Edit"
                });

            // delete roster
            context.MapRoute(
                "DeleteRoster",
                "Roster/Delete/{id}",
                new
                {
                    controller = "Roster",
                    action = "Delete"
                });

            // edit players
            context.MapRoute(
                "EditPlayers",
                "Roster/Players/{id}",
                new
                {
                    controller = "Roster",
                    action = "EditPlayers"
                });

            // view roster
            context.MapRoute(
                "ViewRoster",
                "RosterView/{season}/{turn}",
                new
                {
                    controller = "Roster",
                    action = "View",
                    season = UrlParameter.Optional,
                    turn = UrlParameter.Optional
                });
        }
    }
}
