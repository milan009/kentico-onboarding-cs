using System;
using System.Web.Http;
using System.Web.Http.Routing;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Repositories;
using ListApp.Api.Services;
using ListApp.Utils;
using Microsoft.Practices.Unity;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.AddApiVersioning();

            // Dependency resolver
            var container = new UnityContainer();
            container.RegisterType<IRepository<Guid, ListItem>, ListItemRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IGuidGenerator, GuidGenerator>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes(InitializeConstraintResolver());
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
