using System;
using System.Net.Http;
using System.Web.Http.Routing;
using ListApp.Contracts.Interfaces;

namespace ListApp.Utils.RouteHelper
{
    internal class RouteHelper : IRouteHelper
    {
        private readonly IRouteHelperConfig _config;
        private readonly UrlHelper _helper;    

        public RouteHelper(HttpRequestMessage message, IRouteHelperConfig config)
        {
            _config = config;
            _helper = new UrlHelper(message);
        }

        public string GetItemUrl(Guid id)
        {
            return _helper.Route(_config.ItemsControllerRoute, new {id});
        }
    }
}
