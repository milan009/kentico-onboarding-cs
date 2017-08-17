using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Routing;
using JsonPatch;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            var constraintResolver = new DefaultInlineConstraintResolver()
            {
                ConstraintMap =
                {
                    ["apiVersion"] = typeof( ApiVersionRouteConstraint )
                }
            };

            config.Formatters.Add(new JsonPatchFormatter(new JsonPatchSettings
            {
                PathResolver = new CaseInsensitivePropertyPathResolver()
            }));
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
