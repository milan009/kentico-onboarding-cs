using System.Net.Http;
using System.Web;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using Microsoft.Practices.Unity;
using System;

namespace ListApp.Api.Bootstrapper
{
    internal class ApiBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
        {
            var connectionString = GetDbConnectionStringFromConfig("MongoDBConnectionString");
            if(connectionString == null)
            {
                throw new NullReferenceException("Specified connection string was not found!");
            }

            return container
                .RegisterInstance(new DatabaseConfiguration{ConnectionString = connectionString})
                .RegisterType<IRouteHelperConfig, RouteHelperConfig>(new HierarchicalLifetimeManager())
                .RegisterType<HttpRequestMessage>(
                    new HierarchicalLifetimeManager(),
                    new InjectionFactory(ExtractHttpRequestMessage));
        }

        private HttpRequestMessage ExtractHttpRequestMessage(IUnityContainer container) 
            => (HttpRequestMessage) HttpContext.Current.Items["MS_HttpRequestMessage"];

        private string GetDbConnectionStringFromConfig(string connectionName)
        {
            var connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings;

            var connString = connectionStrings[connectionName];
            return connString?.ConnectionString ?? null;
        }
    }
}