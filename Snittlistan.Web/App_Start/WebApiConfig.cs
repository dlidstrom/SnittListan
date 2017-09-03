﻿using System.Web.Http;
using Newtonsoft.Json.Serialization;
using Snittlistan.Web.Infrastructure;
using Snittlistan.Web.Infrastructure.Attributes;

namespace Snittlistan.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            // camelCase by default
            var formatter = config.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Filters.Add(new UnhandledExceptionFilter());
            config.Formatters.Add(new ICalFormatter());
            config.MessageHandlers.Add(new OutlookAgentMessageHandler());
        }
    }
}