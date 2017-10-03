using System.Net.Http;
using System.Web;
using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Bootstrapper
{
    internal class ApiBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
            => container
            .RegisterType<IRouteHelperConfig, RouteHelperConfig>(new HierarchicalLifetimeManager())
            .RegisterType<HttpRequestMessage>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(ExtractHttpRequestMessage));

        private HttpRequestMessage ExtractHttpRequestMessage(IUnityContainer container) =>
            (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];
    }
}