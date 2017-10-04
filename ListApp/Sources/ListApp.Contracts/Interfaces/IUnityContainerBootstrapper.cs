using Microsoft.Practices.Unity;

namespace ListApp.Contracts.Interfaces
{
    public interface IUnityContainerBootstrapper
    {
        IUnityContainer RegisterTypes(IUnityContainer container);
    }
}