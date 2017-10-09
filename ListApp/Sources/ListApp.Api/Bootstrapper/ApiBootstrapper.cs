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
            .RegisterInstance("connectionString", getDbConnectionStringFromConfig("MongoDBConnectionString"))
            .RegisterType<IRouteHelperConfig, RouteHelperConfig>(new HierarchicalLifetimeManager())
            .RegisterType<HttpRequestMessage>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(ExtractHttpRequestMessage));

        private HttpRequestMessage ExtractHttpRequestMessage(IUnityContainer container) =>
            (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];

        private string getDbConnectionStringFromConfig(string connectionName)
        {
            System.Configuration.Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count <= 0) return null;

            var connString = rootWebConfig.ConnectionStrings.ConnectionStrings[connectionName];
            return connString.ConnectionString;
        }
    }
}