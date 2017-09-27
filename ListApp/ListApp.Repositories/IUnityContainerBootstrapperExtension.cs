using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using Microsoft.Practices.Unity;

namespace ListApp.Repositories
{
    public static class ListItemRepositoryBootstrapperExtension
    {
        public static void RegisterListItemRepository(this IUnityContainer container)
        {
            container.RegisterType<IRepository, ListItemRepository>(new HierarchicalLifetimeManager());
        }
    }
}
