using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.Web.Http.Routing;
using MongoDB.Driver.Core.Configuration;

namespace ListApp.Api
{
    public static class RoutingConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.AddApiVersioning();

            // Dependency resolver
            //var container = new UnityContainer();
            //container.RegisterType<IRepository<Guid, ListItem>, ListItemRepository>(new InjectionConstructor(GetDBConnectionString("MongoDBConnectionString")));
            //container.RegisterType<IGuidGenerator, GuidGenerator>(new HierarchicalLifetimeManager());
            //config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes(InitializeConstraintResolver());
        }

        private static IInlineConstraintResolver InitializeConstraintResolver()
        {
            return new DefaultInlineConstraintResolver
            {
                ConstraintMap =
                {
                    ["apiVersion"] = typeof( ApiVersionRouteConstraint )
                }
            };
        }

        private static string GetDBConnectionString(string connectionName)
        {
            System.Configuration.Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count <= 0) return null;

            var connString = rootWebConfig.ConnectionStrings.ConnectionStrings[connectionName];
            return connString.ConnectionString;
        }
    }
}
