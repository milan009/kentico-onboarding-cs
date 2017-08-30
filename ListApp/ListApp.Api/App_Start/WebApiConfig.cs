using System;
using System.Web.Http;
using System.Web.Http.Routing;
using JsonPatch;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;
using ListApp.Api.Filters;
using ListApp.Api.ModelBinders;
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

            // Guid model binder - the default one was not returning an error,
            // and "null" got passed into not nullable ModelState dictionary value
            // if the passed string was not convertible to a GUID
            config.BindParameter(typeof(Guid), new GuidModelBinder());
            
            // A filter that takes care of "null" arguments
            config.Filters.Add(new NullArgumentActionFilter());

            // A filter that takse care of situations when models are invalid
            config.Filters.Add(new ModelValidationActionFilter());

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
