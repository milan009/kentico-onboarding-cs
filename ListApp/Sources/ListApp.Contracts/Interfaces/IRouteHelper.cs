using System;

namespace ListApp.Api.Services.RouteHelper
{
    public interface IRouteHelper
    {
        string GetItemUrl(Guid id);
    }
}
