﻿namespace Snittlistan.Test
{
    using Castle.Windsor;

    using Snittlistan.Web.Infrastructure.Installers;
    using Snittlistan.Web.Services;

    using Xunit;

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
            var service = container.Resolve<IAuthenticationService>();
            Assert.NotNull(service);
        }

        [Fact]
        public void InstallsEmailService()
        {
            var handlers = InstallerTestHelper.GetHandlersFor(typeof(IEmailService), container);
            Assert.Equal(1, handlers.Length);
        }
    }
}