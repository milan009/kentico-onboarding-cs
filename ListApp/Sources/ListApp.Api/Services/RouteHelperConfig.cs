using ListApp.Api.Controllers.V1;
using ListApp.Contracts.Interfaces;

namespace ListApp.Api.Services
{
    internal class RouteHelperConfig : IRouteHelperConfig
    {
        public string ItemsControllerRoute => ItemsController.RouteName;
    }
}