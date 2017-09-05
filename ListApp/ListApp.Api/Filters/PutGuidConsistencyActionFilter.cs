using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ListApp.Api.Models;

namespace ListApp.Api.Filters
{
    public class PutGuidConsistencyActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if ((Guid)actionContext.ActionArguments["id"] != ((ListItem)actionContext.ActionArguments["newItem"]).Id)
            {
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Inconsistence in URL GUID and ListItem object GUID!");
            }
        }
    }
}