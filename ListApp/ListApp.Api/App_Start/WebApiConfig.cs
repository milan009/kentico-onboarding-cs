using System;
using System.Web.Http;
using System.Web.Http.Routing;
using JsonPatch;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;
using ListApp.Api.Filters;
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

            // A filter that takse care of situations when models are invalid
            config.Filters.Add(new ModelValidationActionFilter());

            // A filter that takes care of "null" arguments
            config.Filters.Add(new NullArgumentActionFilter());

            // A formatter that parses the body of a PATCH request 
            config.Formatters.Add(new JsonPatchFormatter(new JsonPatchSettings
            {
                PathResolver = new CaseInsensitivePropertyPathResolver()
            }));

            // The versioning plugin
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
