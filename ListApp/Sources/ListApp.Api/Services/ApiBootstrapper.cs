using System.Net.Http;
using System.Web;
using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Services
{
    internal class ApiBootstrapper : IUnityContainerBootstrapper
    {
        private void RegisterRouteHelperConfig(IUnityContainer container)
        {
            container.RegisterType<IRouteHelperConfig, ItemsRouteHelperConfig>(new HierarchicalLifetimeManager());
        }

        private void RegisterHttpRequestMessage(IUnityContainer container)
        {
            container.RegisterType<HttpRequestMessage>(new HierarchicalLifetimeManager(),
                new InjectionFactory( _ => (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"]));
        }

        public IUnityContainer RegisterTypes(IUnityContainer container)
        {
            RegisterRouteHelperConfig(container);
            RegisterHttpRequestMessage(container);

            return container;
        }
    }
}