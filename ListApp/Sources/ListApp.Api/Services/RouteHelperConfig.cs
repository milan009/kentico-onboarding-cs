using ListApp.Api.Controllers.V1;
using ListApp.Contracts.Interfaces;

namespace ListApp.Api.Services
{
    internal class RouteHelperConfig : IRouteHelperConfig
    {
        public ItemsRouteHelperConfig()
        {
            ItemsControllerRoute = ItemsController.RouteName;
        }

        public string ItemsControllerRoute { get; set; }
    }
}