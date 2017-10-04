using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Services.GuidGenerator
{
    public class GuidGeneratorBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
        {
            return container.RegisterType<IGuidGenerator, GuidGenerator>(new HierarchicalLifetimeManager());
        }
    }
}
