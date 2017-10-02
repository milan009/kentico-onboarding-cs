using System.Web.Http;
using ListApp.Api.Utils;
using ListApp.Repositories;
using Microsoft.Practices.Unity;

namespace ListApp.Api
{
    internal class DependencyResolverConfig
    {
        internal static void Register(HttpConfiguration config)
        {
            var container = new UnityContainer();

            var listItemRepositoryBootstrapper = new ListItemRepositoryBootstrapper();
            listItemRepositoryBootstrapper.RegisterListItemRepository(container);
            
            config.DependencyResolver = new UnityResolver(container);
        }
    }
}