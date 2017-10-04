using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Repositories.Bootstrapper
{
    public class ListItemRepositoryBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
            => container
                .RegisterType<IRepository, ListItemRepository>(
                    new HierarchicalLifetimeManager(), 
                    new InjectionConstructor(new ResolvedParameter<string>("connectionString")));
    }
}
