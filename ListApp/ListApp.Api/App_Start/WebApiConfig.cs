using System;
using System.Web.Http;
using System.Web.Http.Routing;
using ListApp.Api.Interfaces;
using ListApp.Api.Models;
using ListApp.Api.Repositories;
using ListApp.Api.Services;
using ListApp.Api.Utils;
using Microsoft.Practices.Unity;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Dependency resolver
            var container = new UnityContainer();
            container.RegisterType<IRepository<Guid, ListItem>, ListItemRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<IGuidGenerator, GuidGenerator>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);


            var constraintResolver = new DefaultInlineConstraintResolver
            {
                ConstraintMap =
                {
                    ["apiVersion"] = typeof( ApiVersionRouteConstraint )
                }
            };

            config.AddApiVersioning();

            // Web API routes
            config.MapHttpAttributeRoutes(constraintResolver);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v{version:apiVersion}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
