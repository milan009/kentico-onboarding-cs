﻿using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Web.Http.Routing;

namespace ListApp.Api
{
    internal static class RoutingConfig
    {
        internal static void Register(HttpConfiguration config)
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
