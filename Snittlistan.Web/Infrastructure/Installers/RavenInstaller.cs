﻿#nullable enable

namespace Snittlistan.Web.Infrastructure.Installers
{
    using System;
    using System.Reflection;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using NLog;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Embedded;
    using Snittlistan.Queue.Models;
    using Snittlistan.Web.Areas.V1.Models;

    public class RavenInstaller : IWindsorInstaller
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly SiteWideConfiguration? siteWideConfiguration;
        private readonly DocumentStoreMode mode;

        public RavenInstaller(SiteWideConfiguration siteWideConfiguration)
        {
            this.siteWideConfiguration = siteWideConfiguration;
            mode = MvcApplication.Mode switch
            {
                ApplicationMode.Debug => DocumentStoreMode.Server,
                ApplicationMode.Release => DocumentStoreMode.Server,
                ApplicationMode.Test => DocumentStoreMode.InMemory,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public RavenInstaller(DocumentStoreMode mode)
        {
            this.mode = mode;
        }

        public void Install(IWindsorContainer container, IConfigurationStore configurationStore)
        {
            if (mode == DocumentStoreMode.InMemory)
            {
                EmbeddableDocumentStore store = new()
                {
                    RunInMemory = true
                };
                store.Configuration.Storage.Voron.AllowOn32Bits = true;
                _ = container.Register(Component.For<IDocumentStore>().Instance(store.Initialize()));
#pragma warning disable 618
                store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
#pragma warning restore 618
            }
            else
            {
                TenantConfiguration[] tenantConfigurations = container.ResolveAll<TenantConfiguration>();
                foreach (TenantConfiguration tenantConfiguration in tenantConfigurations)
                {
                    IDocumentStore documentStore = InitializeStore(CreateDocumentStore(tenantConfiguration));
                    string nameOfComponent = $"DocumentStore-{tenantConfiguration.Hostname}";
                    Log.Info($"Registering document store named {nameOfComponent}");
                    ComponentRegistration<IDocumentStore> documentStoreComponent = Component.For<IDocumentStore>()
                        .Instance(documentStore)
                        .Named(nameOfComponent)
                        .LifestyleSingleton();
                    container.Register(documentStoreComponent);
                }
            }

            if (mode == DocumentStoreMode.InMemory)
            {
                container.Register(
                    Component.For<IDocumentSession>().UsingFactoryMethod(GetDocumentSession).LifestyleScoped());
            }
            else
            {
                container.Register(
                    Component.For<IDocumentSession>().UsingFactoryMethod(GetDocumentSession).LifestylePerWebRequest());
            }
        }

        private static bool FindIdentityProperty(MemberInfo memberInfo)
        {
            var attribute = Attribute.GetCustomAttribute(memberInfo, typeof(IdAttribute));
            return attribute != null || memberInfo.Name == "Id";
        }

        private IDocumentSession GetDocumentSession(IKernel kernel)
        {
            // document store is resolved depending on hostname
            IDocumentStore store = kernel.Resolve<IDocumentStore>();
            IDocumentSession documentSession;
            if (mode == DocumentStoreMode.InMemory)
            {
                documentSession = store.OpenSession();
            }
            else
            {
                documentSession = store.OpenSession();
            }

            documentSession.Advanced.UseOptimisticConcurrency = true;
            return documentSession;
        }

        private static IDocumentStore InitializeStore(IDocumentStore store)
        {
            store.Initialize();
            store.Conventions.IdentityPartsSeparator = "-";
#pragma warning disable 618
            store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
#pragma warning restore 618
            store.Conventions.FindTypeTagName = type => type == typeof(Match8x4) ? "Matches" : DocumentConvention.DefaultTypeTagName(type);
            store.Conventions.FindIdentityProperty = FindIdentityProperty;
            store.Conventions.MaxNumberOfRequestsPerSession = 1024;

            return store;
        }

        private IDocumentStore CreateDocumentStore(TenantConfiguration tenantConfiguration)
        {
            IDocumentStore store;
            switch (mode)
            {
                case DocumentStoreMode.InMemory:
                    {
                        var documentStore = new EmbeddableDocumentStore
                        {
                            RunInMemory = true
                        };

                        store = documentStore;
                    }

                    break;

                case DocumentStoreMode.Server:
                    store = new DocumentStore
                    {
                        Url = siteWideConfiguration.DatabaseUrl,
                        DefaultDatabase = tenantConfiguration.DatabaseName
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return store;
        }
    }
}
