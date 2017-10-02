﻿using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Repositories
{
    public static class UnityContainerBootstrapperExtension
    {
        public static IUnityContainer RegisterListItemRepository(this IUnityContainer container)
            => container
                .RegisterType<IRepository, ListItemRepository>(new HierarchicalLifetimeManager());
    }
}
