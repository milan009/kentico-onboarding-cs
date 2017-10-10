using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Services.Bootstrapper
{
    public class ServicesApiBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container) =>
            container
                .RegisterType<IRouteHelper, RouteHelper.RouteHelper>(new HierarchicalLifetimeManager());
    }
}