namespace Snittlistan.Test
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;
    using Moq;
    using MvcContrib.TestHelper;

    public static class RouteTestHelper
    {
        public static void Maps(this RouteCollection routes, string httpVerb, string url, object expectations)
        {
            var routeData = RetrieveRouteData(routes, httpVerb, url);
            routeData.ShouldNotBeNull("Should have found the route");

            foreach (var property in GetProperties(expectations))
            {
                var equal = string.Equals(
                    property.Value.ToString(),
                    routeData.Values[property.Name].ToString(),
                    StringComparison.OrdinalIgnoreCase);
                var message = string.Format("Expected '{0}', not '{1}' for '{2}'.", property.Value, routeData.Values[property.Name], property.Name);
                equal.ShouldEqual(true, message);
            }
        }

        public static void DoNotMap(this RouteCollection routes, string httpVerb, string url)
        {
            RetrieveRouteData(routes, httpVerb, url)
                .ShouldEqual((RouteData)null, "Should not find a route");
        }

        private static RouteData RetrieveRouteData(RouteCollection routes, string httpVerb, string url)
        {
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns(url);
            httpContext.Setup(c => c.Request.HttpMethod).Returns(httpVerb);

            return routes.GetRouteData(httpContext.Object);
        }

        private static IEnumerable<PropertyValue> GetProperties(object o)
        {
            return from prop in TypeDescriptor.GetProperties(o).Cast<PropertyDescriptor>()
                   let val = prop.GetValue(o)
                   where val != null
                   select new PropertyValue { Name = prop.Name, Value = val };
        }

        private sealed class PropertyValue
        {
            public string Name
            {
                get;
                set;
            }

            public object Value
            {
                get;
                set;
            }
        }
    }
}