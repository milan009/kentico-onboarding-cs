using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ListApp.Api.Models;

namespace ListApp.Api.Filters
{
    public class PutCollectionActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var listItems = (IEnumerable<ListItem>) actionContext.ActionArguments["items"];
            var enumerable = listItems as ListItem[] ?? listItems.ToArray();

            if (!enumerable.Any())
            {
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Putting empty collection is not allowed!");
            }

            var diffCheck = new HashSet<Guid>();
            if (!enumerable.All(item => diffCheck.Add(item.Id)))
            {
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GUIDs in given collection are not unique!");
            } 
        }
    }
}