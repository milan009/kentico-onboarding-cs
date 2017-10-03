using System.Web.Http;
using ListApp.Api.Services;
using ListApp.Api.Utils;
using ListApp.Contracts.Interfaces;
using ListApp.Repositories;
using ListApp.Repositories.Bootstrapper;
using ListApp.Utils.RouteHelper;
using Microsoft.Practices.Unity;

namespace ListApp.Api
{
    internal static class DependencyResolverConfig
    {
        internal static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer()
                .ExecuteBootstrapper<ListItemRepositoryBootstrapper>()
                .ExecuteBootstrapper<RouteHelperBootstrapper>()
                .ExecuteBootstrapper<ApiBootstrapper>();

            config.DependencyResolver = new UnityResolver(container);
        }

        private static IUnityContainer ExecuteBootstrapper<TBootstrapper>(this IUnityContainer container)
            where TBootstrapper : IUnityContainerBootstrapper, new()
                => new TBootstrapper().RegisterTypes(container);
    }
}