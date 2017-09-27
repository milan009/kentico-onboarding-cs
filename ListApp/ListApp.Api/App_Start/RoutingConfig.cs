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
