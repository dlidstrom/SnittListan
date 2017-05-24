﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Snittlistan.Web.Areas.V2.Migration;

namespace Snittlistan.Web.Infrastructure.Installers
{
    public class EventMigratorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                AllTypes.FromThisAssembly()
                    .BasedOn<IEventMigratorWithResults>()
                    .WithServiceFromInterface(typeof(IEventMigratorWithResults))
                    .LifestyleTransient());
        }
    }
}