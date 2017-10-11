using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api
{
    internal static class RoutingConfig
    {
        internal static void Register(HttpConfiguration config)
        {
            config.AddApiVersioning();

            config.MapHttpAttributeRoutes(InitializeConstraintResolver());
        }

        private static IInlineConstraintResolver InitializeConstraintResolver()
        {
            return new DefaultInlineConstraintResolver
            {
                ConstraintMap =
                {
                    ["apiVersion"] = typeof(ApiVersionRouteConstraint)
                }
            };
        }
    }
}