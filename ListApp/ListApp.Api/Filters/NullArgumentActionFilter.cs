using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ListApp.Api.Filters
{
    public class NullArgumentActionFilter : ActionFilterWithInjectibleRequest
    {
        public NullArgumentActionFilter(HttpRequestMessage testMessage) : base(testMessage) { }

        public NullArgumentActionFilter() { }

        protected override void DoValidation(HttpActionContext actionContext, HttpRequestMessage request)
        {
            if (actionContext.ActionArguments.ContainsValue(null))
            {
                actionContext.Response =
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, "A null argument is not allowed!");
            }
        }
    }
}