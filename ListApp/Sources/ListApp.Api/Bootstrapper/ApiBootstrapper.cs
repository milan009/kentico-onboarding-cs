using System.Net.Http;
using System.Web;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Bootstrapper
{
    internal class ApiBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
            => container
                .RegisterInstance(new DatabaseConfiguration
                {
                    ConnectionString = _getDbConnectionStringFromConfig("MongoDBConnectionString")
                })
                .RegisterType<IRouteHelperConfig, RouteHelperConfig>(new HierarchicalLifetimeManager())
                .RegisterType<HttpRequestMessage>(
                    new HierarchicalLifetimeManager(),
                    new InjectionFactory(ExtractHttpRequestMessage));

        private HttpRequestMessage ExtractHttpRequestMessage(IUnityContainer container) 
            => (HttpRequestMessage) HttpContext.Current.Items["MS_HttpRequestMessage"];

        private string _getDbConnectionStringFromConfig(string connectionName)
        {
            var rootWebConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            var connectionStrings = rootWebConfig.ConnectionStrings.ConnectionStrings;

            if (connectionStrings.Count <= 0)
            {
                return null;
            }

            var connString = connectionStrings[connectionName];
            return connString.ConnectionString;
        }
    }
}