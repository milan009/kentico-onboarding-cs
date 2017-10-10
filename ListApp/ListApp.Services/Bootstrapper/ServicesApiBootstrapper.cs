using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Services.Bootstrapper
{
    public class ServicesBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container) =>
            container
                .RegisterType<IGuidGenerator, GuidGenerator.GuidGenerator>(new TransientLifetimeManager())
                .RegisterType<ITimeHelper, TimeHelper.TimeHelper>(new TransientLifetimeManager());
    }
}