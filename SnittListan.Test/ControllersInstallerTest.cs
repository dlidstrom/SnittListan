﻿using System;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.Windsor;
using SnittListan.Controllers;
using SnittListan.Installers;
using Xunit;
using Castle.Core;

namespace SnittListan.Test
{
	public class ControllersInstallerTest
	{
		private readonly IWindsorContainer container;

		public ControllersInstallerTest()
		{
			container = new WindsorContainer()
				.Install(new ControllerInstaller());
		}

		[Fact]
		public void AllControllersImplementIController()
		{
			var allHandlers = InstallerTestHelper.GetAllHandlers(container);
			var controllerHandlers = InstallerTestHelper.GetHandlersFor(typeof(IController), container);
			Assert.NotEmpty(allHandlers);
			Assert.Equal(allHandlers, controllerHandlers);
		}

		[Fact]
		public void AllControllersAreRegistered()
		{
			// Is<TType> is a helper extension method from Windsor
			// which behaves like 'is' keyword in C# but at a Type, not instance level

			var allControllers = InstallerTestHelper.GetPublicClassesFromApplicationAssembly(c => c.Is<IController>());
			var registeredControllers = InstallerTestHelper.GetImplementationTypesFor(typeof(IController), container);
			Assert.Equal(allControllers, registeredControllers);
		}

		[Fact]
		public void AllAndOnlyControllersHaveControllerSuffix()
		{
			var allControllers = InstallerTestHelper.GetPublicClassesFromApplicationAssembly(c => c.Name.EndsWith("Controller"));
			var registeredControllers = InstallerTestHelper.GetImplementationTypesFor(typeof(IController), container);
			Assert.Equal(allControllers, registeredControllers);
		}

		[Fact]
		public void AllAndOnlyControllersLiveInControllersNamespace()
		{
			var allControllers = InstallerTestHelper.GetPublicClassesFromApplicationAssembly(c => c.Namespace.Contains("Controllers"));
			var registeredControllers = InstallerTestHelper.GetImplementationTypesFor(typeof(IController), container);
			Assert.Equal(allControllers, registeredControllers);
		}

		[Fact]
		public void AllControllersAreTransient()
		{
			var nonTransientControllers = InstallerTestHelper.GetHandlersFor(typeof(IController), container)
				.Where(c => c.ComponentModel.LifestyleType != LifestyleType.Transient)
				.ToArray();
			Assert.Empty(nonTransientControllers);
		}

		[Fact]
		public void AllControllersExposeThemselvesAsService()
		{
			var controllersWithWrongName = InstallerTestHelper.GetHandlersFor(typeof(IController), container)
				.Where(c => c.Service != c.ComponentModel.Implementation)
				.ToArray();
			Assert.Empty(controllersWithWrongName);
		}
	}
}
