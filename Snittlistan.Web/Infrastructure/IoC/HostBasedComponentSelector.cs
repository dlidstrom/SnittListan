﻿namespace Snittlistan.Web.Infrastructure.IoC
{
    using System;
    using System.Linq;
    using System.Web;
    using Castle.MicroKernel;
    using Raven.Client;
    using Snittlistan.Queue.Infrastructure;

    public class HostBasedComponentSelector : IHandlerSelector
    {
        public bool HasOpinionAbout(string key, Type service)
        {
            try
            {
                bool result = service == typeof(Tenant)
                    || service == typeof(IDocumentStore);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            string hostname = GetHostname();
            if (service == typeof(IDocumentStore))
            {
                hostname = $"DocumentStore-{hostname}";
            }

            IHandler selectedHandler = handlers.SingleOrDefault(h => h.ComponentModel.Name == hostname);
            if (selectedHandler == null)
            {
                throw new Exception($"No {service} configured with name {hostname}");
            }

            return selectedHandler;
        }

        private static string GetHostname()
        {
            return HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
        }
    }
}
