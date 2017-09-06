using System.Web.Http;
using System.Web.Http.Routing;
using JsonPatch;
using JsonPatch.Formatting;
using JsonPatch.Paths.Resolvers;
using ListApp.Api.Filters;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api.Utils
{
    public static class ServerInit
    {
        public static HttpConfiguration InitializeConfiguration(HttpConfiguration config)
        {
            var constraintResolver = new DefaultInlineConstraintResolver()
            {
                ConstraintMap =
                {
                    ["apiVersion"] = typeof( ApiVersionRouteConstraint )
                }
            };

            //   config.BindParameter(typeof(Guid), new GuidModelBinder());
            config.Filters.Add(new ModelValidationActionFilter());
            config.Filters.Add(new NullArgumentActionFilter());
            config.Formatters.Add(new JsonPatchFormatter(new JsonPatchSettings
            {
                PathResolver = new CaseInsensitivePropertyPathResolver()
            }));
            config.AddApiVersioning();
            config.MapHttpAttributeRoutes(constraintResolver);
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v{version:apiVersion}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            return config;
        }
    }
}