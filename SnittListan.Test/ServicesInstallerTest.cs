﻿using Castle.Windsor;
using SnittListan.Installers;
using SnittListan.Services;
using Xunit;

namespace SnittListan.Test
{
	public class ServicesInstallerTest
	{
		private readonly IWindsorContainer container;

		public ServicesInstallerTest()
		{
			container = new WindsorContainer().Install(new ServicesInstaller());
		}

		[Fact]
		public void InstallsFormsAuthenticationService()
		{
			Assert.NotNull(container.Resolve<IFormsAuthenticationService>());
		}

		[Fact]
		public void InstallsEmailService()
		{
			Assert.NotNull(container.Resolve<IEmailService>());
		}
	}
}
