﻿namespace Snittlistan.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        private readonly RouteCollection routes;

        public RouteConfig(RouteCollection routes)
        {
            this.routes = routes;
        }

        public void Configure()
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            // robots.txt
            routes.IgnoreRoute("{file}.txt");

            routes.MapRoute(
                name: "Hacker-Routes",
                url: "{*php}",
                defaults: new { controller = "Hacker", action = "Index" },
                constraints: new { php = @"cpanel|status|.*php.*|catalog|^s?cgi(\-bin)?.*|^scripts.*|^(aw)?stats.*|^shop.*|feed.*|temp.*|console" });
        }
    }
}