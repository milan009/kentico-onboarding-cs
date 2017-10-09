using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Services.Bootstrapper
{
    public class ServicesApiBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
        {
            return container
                .RegisterType<IGuidGenerator, GuidGenerator.GuidGenerator>(new HierarchicalLifetimeManager())
                .RegisterType<IRouteHelper, RouteHelper.RouteHelper>(new HierarchicalLifetimeManager());
        }
    }
}
