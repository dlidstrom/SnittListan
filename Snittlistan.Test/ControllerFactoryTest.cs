﻿using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using NUnit.Framework;
using Snittlistan.Web.Areas.V1.Controllers;
using Snittlistan.Web.Infrastructure.Installers;
using Snittlistan.Web.Infrastructure.IoC;

namespace Snittlistan.Test
{
    [TestFixture]
    public class ControllerFactoryTest
    {
        [Test]
        public void CanCreateController()
        {
            var container = new WindsorContainer()
                .Install(new ControllerFactoryInstaller())
                .Install(new ControllerInstaller());

            IControllerFactory factory = null;
            Assert.DoesNotThrow(() => factory = container.Resolve<IControllerFactory>());
            IController controller = null;
            Assert.DoesNotThrow(() => controller = factory.CreateController(new RequestContext(), typeof(ErrorController).Name.Replace("Controller", string.Empty)));
            Assert.NotNull(controller);
        }
    }
}