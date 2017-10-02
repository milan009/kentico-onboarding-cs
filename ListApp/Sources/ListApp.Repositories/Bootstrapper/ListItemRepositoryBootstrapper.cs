using ListApp.Contracts.Interfaces;
using ListApp.Repositories.Bootstrapper;
using Microsoft.Practices.Unity;

namespace ListApp.Repositories
{
    public class ListItemRepositoryBootstrapper : IBootstrapper
    {
        public IUnityContainer RegisterListItemRepository(IUnityContainer container)
            => container
                .RegisterType<IRepository, ListItemRepository>(new HierarchicalLifetimeManager());
    }
}
