using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api
{
    public static class RoutingConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.AddApiVersioning();

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
