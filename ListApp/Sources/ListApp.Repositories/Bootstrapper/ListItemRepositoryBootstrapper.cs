using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Repositories.Bootstrapper
{
    public class ListItemRepositoryBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
            => container
                .RegisterType<IListItemRepository, ListItemListItemRepository>(new ContainerControlledLifetimeManager());
    }
}