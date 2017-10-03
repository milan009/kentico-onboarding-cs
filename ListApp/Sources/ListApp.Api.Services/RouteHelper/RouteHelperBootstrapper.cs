using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Services.RouteHelper
{
    public class RouteHelperBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container) => 
            container.RegisterType<IRouteHelper, RouteHelper>(new HierarchicalLifetimeManager());
    }
}