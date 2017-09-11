using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace ListApp.Api.Filters
{
    public class ModelValidationActionFilter : ActionFilterWithInjectibleRequest
    {
        public ModelValidationActionFilter() { }

        public ModelValidationActionFilter(HttpRequestMessage testMessage) : base(testMessage) { }

        protected override void DoValidation(HttpActionContext actionContext, HttpRequestMessage request)
        {
            var modelState = actionContext.ModelState;

            if (!modelState.IsValid)
            {
                actionContext.Response =
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, modelState);
            }
        }
    }
}