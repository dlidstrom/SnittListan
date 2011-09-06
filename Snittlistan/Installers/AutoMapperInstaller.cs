﻿using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace SnittListan.Installers
{
	public class AutoMapperInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(FindProfiles().LifestyleSingleton());
		}

		private static BasedOnDescriptor FindProfiles()
		{
			return AllTypes
				.FromThisAssembly()
				.BasedOn<Profile>();
		}
	}
}