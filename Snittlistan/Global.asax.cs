﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Common.Logging;
using Snittlistan.Helpers;
using Snittlistan.Helpers.Attributes;
using Snittlistan.Infrastructure;
using Snittlistan.IoC;

namespace Snittlistan
{
	public class MvcApplication : HttpApplication
	{
		private static readonly ILog logger = LogManager.GetCurrentClassLogger();
		private static IWindsorContainer container;

		public static IWindsorContainer Container
		{
			get { return container; }
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new ElmahHandleErrorAttribute());
			filters.Add(new HandleErrorAttribute());
			filters.Add(new RavenActionFilterAttribute());
		}

		protected void Application_Start()
		{
			logger.Info("Application Starting");
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);

			// initialize container and controller factory
			InitializeContainer();

			// register routes
			new RouteConfigurator(RouteTable.Routes).Configure();

			// add model binders
			ModelBinders.Binders.Add(typeof(Guid), new GuidBinder());

			// configure AutoMapper
			AutoMapperConfiguration.Configure(container);
		}

		protected void Application_End()
		{
			logger.Info("Application Ending");
			container.Dispose();
		}

		private void InitializeContainer()
		{
			if (container == null)
			{
				container = new WindsorContainer()
					.Install(FromAssembly.This());
			}

			var controllerFactory = new WindsorControllerFactory(container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}
	}
}