using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using JsonPatch;
using ListApp.Api.Models;

namespace ListApp.Api.Filters
{
    public class PatchSingleResourceActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var thePatch = (JsonPatch.JsonPatchDocument<ListItem>) actionContext.ActionArguments["patch"];

            if (!thePatch.HasOperations)
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                        "No operation was found in the patch object.");

            // Only replace operation on "/Text" is allowed
            if (!thePatch.Operations.Select(op => op.Operation == JsonPatchOperationType.replace && op.Path.ToLower() == "/text")
                .Aggregate((a, b) => a && b))
            {
                actionContext.Response =
                    actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden,
                        "Only replace operations on the \"Text\" field are allowed!");
            }
        }
    }
}