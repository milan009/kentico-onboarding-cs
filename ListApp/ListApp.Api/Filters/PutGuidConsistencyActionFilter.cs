using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using ListApp.Api.Models;

namespace ListApp.Api.Filters
{
    public class PutGuidConsistencyActionFilter : ActionFilterWithInjectibleRequest
    {
        public PutGuidConsistencyActionFilter() { }

        public PutGuidConsistencyActionFilter(HttpRequestMessage testMessage) : base(testMessage) { }

        protected override void DoValidation(HttpActionContext actionContext, HttpRequestMessage request)
        {
            if ((Guid)actionContext.ActionArguments["id"] != ((ListItem)actionContext.ActionArguments["newItem"]).Id)
            {
                actionContext.Response =
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, "Inconsistence in URL GUID and ListItem object GUID!");
            }
        }
    }
}