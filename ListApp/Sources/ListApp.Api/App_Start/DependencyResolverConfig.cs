﻿using System.Web.Http;
using ListApp.Api.Bootstrapper;
using ListApp.Api.Services.Bootstrapper;
using ListApp.Api.Utils;
using ListApp.Contracts.Interfaces;
using ListApp.Repositories.Bootstrapper;
using ListApp.Services.Bootstrapper;
using Microsoft.Practices.Unity;

namespace ListApp.Api
{
    internal static class DependencyResolverConfig
    {
        internal static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer()
                .ExecuteBootstrapper<ListItemRepositoryBootstrapper>()
                .ExecuteBootstrapper<ServicesApiBootstrapper>()
                .ExecuteBootstrapper<ServicesBootstrapper>()
                .ExecuteBootstrapper<ApiBootstrapper>();

            config.DependencyResolver = new UnityResolver(container);
        }

        private static IUnityContainer ExecuteBootstrapper<TBootstrapper>(this IUnityContainer container)
            where TBootstrapper : IUnityContainerBootstrapper, new()
                => new TBootstrapper().RegisterTypes(container);
    }
}