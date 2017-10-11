using ListApp.Contracts.Interfaces;
using ListApp.Services.ItemServices;
using Microsoft.Practices.Unity;

namespace ListApp.Services.Bootstrapper
{
    public class ServicesBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container) =>
            container
                .RegisterType<IGuidGenerator, GuidGenerator.GuidGenerator>(new TransientLifetimeManager())
                .RegisterType<ITimeHelper, TimeHelper.TimeHelper>(new TransientLifetimeManager())
                .RegisterType<IInsertItemService, InsertItemService>(new HierarchicalLifetimeManager())
                .RegisterType<IDeleteItemService, DeleteItemService>(new HierarchicalLifetimeManager())
                .RegisterType<IUpdateItemService, UpdateItemService>(new HierarchicalLifetimeManager());
    }
}