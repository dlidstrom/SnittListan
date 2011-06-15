﻿using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SnittListan.Controllers;

namespace SnittListan.Installers
{
	public class ControllerInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(FindControllers().Configure(ConfigureControllers()));
		}

		private static ConfigureDelegate ConfigureControllers()
		{
			return c => c.LifeStyle.Transient;
		}

		private static BasedOnDescriptor FindControllers()
		{
			return AllTypes
				.FromThisAssembly()
				.BasedOn<IController>()
				.If(Component.IsInSameNamespaceAs<HomeController>())
				.If(t => t.Name.EndsWith("Controller"));
		}
	}
}