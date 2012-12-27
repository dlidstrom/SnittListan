﻿namespace Snittlistan.Web.Infrastructure.Installers
{
    using System.Web.Mvc;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class ControllerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(FindControllers().LifestyleTransient());
        }

        private static BasedOnDescriptor FindControllers()
        {
            return AllTypes
                .FromThisAssembly()
                .BasedOn<IController>()
                .If(t => t.Name.EndsWith("Controller"))
                .Configure(c => c.Named(c.Implementation.Name));
        }
    }
}