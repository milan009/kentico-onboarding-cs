using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using ListApp.Api.Utils;
using ListApp.Repositories;
using Microsoft.Practices.Unity;

namespace ListApp.Api
{
    public class DependencyResolverConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Dependency resolver
            var container = new UnityContainer();
            //container.RegisterType<IRepository<Guid, ListItem>, ListItemRepository>(new HierarchicalLifetimeManager());
            container.RegisterListItemRepository();
            config.DependencyResolver = new UnityResolver(container);
        }

    }
}