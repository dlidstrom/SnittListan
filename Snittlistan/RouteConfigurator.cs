﻿namespace Snittlistan
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Snittlistan.Infrastructure;

    public class RouteConfigurator
    {
        private readonly RouteCollection routes;

        public RouteConfigurator(RouteCollection routes)
        {
            this.routes = routes;
        }

        public void Configure()
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            // robots.txt
            routes.IgnoreRoute("{file}.txt");

            ConfigureAccount();
            ConfigureHome();
            ConfigureElmah();
            ConfigureHacker();
            GoogleRedirects();

            // default route
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // Parameter defaults

            RouteTable.Routes.MapRoute(
                            "NotFound",
                            "notfound",
                            new { controller = "NotFound", action = "NotFound" });
            RouteTable.Routes.MapRoute(
                            "NotFound-Catch-All",
                            "{*any}",
                            new { controller = "NotFound", action = "NotFound" });
        }

        private void ConfigureAccount()
        {
            // ~/logon|register|verify
            routes.MapRouteLowerCase(
                "AccountController-Action",
                "{action}",
                new { controller = "Account" },
                new { action = "^LogOn$|^Register$|^Verify$" });
        }

        private void ConfigureHome()
        {
            // ~/about
            routes.MapRouteLowerCase(
                "HomeController-Action",
                "{action}",
                new { controller = "Home" },
                new { action = "About" });
        }

        private void ConfigureElmah()
        {
            routes.MapRouteLowerCase(
                "ElmahController-internal",
                "admin/elmah/{type}",
                new { controller = "Elmah", action = "Index", type = UrlParameter.Optional });
        }

        private void ConfigureHacker()
        {
            routes.MapRoute(
                "Hacker-Routes",
                "{*php}",
                new { controller = "Hacker", action = "Index" },
                new { php = @".*\.php.*|catalog|^s?cgi(\-bin)?.*|^scripts.*|^(aw)?stats.*|^shop.*" });
        }

        private void GoogleRedirects()
        {
            routes.MapRoute(
                "Google-Redirects",
                "dlcoubfux.html",
                new { controller = "Google", action = "Index" });
        }
    }
}