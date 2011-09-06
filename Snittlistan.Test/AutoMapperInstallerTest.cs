﻿using AutoMapper;
using Castle.Core.Internal;
using Castle.Windsor;
using Snittlistan.Installers;
using Xunit;

namespace Snittlistan.Test
{
	public class AutoMapperInstallerTest
	{
		private readonly IWindsorContainer container;

		public AutoMapperInstallerTest()
		{
			container = new WindsorContainer()
				.Install(new AutoMapperInstaller());
		}

		[Fact]
		public void AllProfilesAreRegistered()
		{
			var allControllers = InstallerTestHelper.GetPublicClassesFromApplicationAssembly(c => c.Is<Profile>());
			var registeredProfiles = InstallerTestHelper.GetImplementationTypesFor(typeof(Profile), container);
			Assert.Equal(allControllers, registeredProfiles);
		}
	}
}
