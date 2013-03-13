﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Snittlistan.Web.Handlers;

namespace Snittlistan.Web.Infrastructure.Installers
{
    public class HandlersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                AllTypes.FromThisAssembly()
                .BasedOn(typeof(IHandle<>))
                .WithServiceFromInterface(typeof(IHandle<>))
                .If(Component.IsInNamespace("Snittlistan.Web.Handlers"))
                .Configure(c => c.LifestyleTransient()));
        }
    }
}