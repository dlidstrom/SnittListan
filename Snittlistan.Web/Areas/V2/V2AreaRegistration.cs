﻿namespace Snittlistan.Web.Areas.V2
{
    using System.Web.Http;
    using System.Web.Mvc;

    public class V2AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "V2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("SessionApi-route", "api/session", new { controller = "SessionApi", action = "Session" });
            context.MapRoute("TurnsApi-route", "api/turns", new { controller = "TurnsApi", action = "Turns" });

            // root routes
            context.MapRoute(
                "V2_root",
                "{action}/{id}",
                new { controller = "App", id = RouteParameter.Optional },
                new { action = "^players$|^results$" });

            // To allow IIS to execute "/notfound" when requesting something which is disallowed,
            // such as /bin or /add_data.
            //context.MapRoute(
            //    name: "NotFound",
            //    url: "notfound",
            //    defaults: new { controller = "NotFound", action = "NotFound" });

            context.MapRoute(
                name: "Hacker-Routes",
                url: "{*php}",
                defaults: new { controller = "Hacker", action = "Index" },
                constraints: new { php = @".*\.php.*|catalog|^s?cgi(\-bin)?.*|^scripts.*|^(aw)?stats.*|^shop.*" });

            // default route
            context.MapRoute(
                "V2_default",
                "{controller}/{action}/{id}",
                new { controller = "App", action = "Index", id = RouteParameter.Optional });

            //context.MapRoute(
            //    name: "NotFound-Catch-All",
            //    url: "{*any}",
            //    defaults: new { controller = "NotFound", action = "NotFound" });
        }
    }
}
