using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListApp.Api.Services.GuidGenerator;
using ListApp.Api.Services.RouteHelper;
using ListApp.Contracts.Interfaces;
using Microsoft.Practices.Unity;

namespace ListApp.Api.Services.Bootstrapper
{
    public class ServicesApiBootstrapper : IUnityContainerBootstrapper
    {
        public IUnityContainer RegisterTypes(IUnityContainer container)
        {
            return container
                .RegisterType<IGuidGenerator, GuidGenerator.GuidGenerator>(new HierarchicalLifetimeManager())
                .RegisterType<IRouteHelper, RouteHelper.RouteHelper>(new HierarchicalLifetimeManager());
        }
    }
}
