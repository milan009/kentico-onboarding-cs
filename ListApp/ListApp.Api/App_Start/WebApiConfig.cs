using System;
using System.Web.Http;
using System.Web.Http.Routing;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Repositories;

using ListApp.Api.Utils;
using Microsoft.Practices.Unity;
using Microsoft.Web.Http.Routing;
using Newtonsoft.Json.Serialization;

namespace ListApp.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // todo: decompose into multiple configs
            // Web API configuration and services
            config.AddApiVersioning();

            // Dependency resolver
            var container = new UnityContainer();
            //container.RegisterType<IRepository<Guid, ListItem>, ListItemRepository>(new HierarchicalLifetimeManager());
            container.RegisterListItemRepository();
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes(InitializeConstraintResolver());

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private static IInlineConstraintResolver InitializeConstraintResolver() => new DefaultInlineConstraintResolver
        {
            ConstraintMap =
            {
                ["apiVersion"] = typeof( ApiVersionRouteConstraint )
            }
        };
    }
}
