using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using ListApp.Api.Models;

namespace ListApp.Api.Filters
{
    public class PutCollectionActionFilter : ActionFilterWithInjectibleRequest
    {
        public PutCollectionActionFilter(HttpRequestMessage testMessage) : base(testMessage) { }

        public PutCollectionActionFilter() { }

        protected override void DoValidation(HttpActionContext actionContext, HttpRequestMessage request)
        {
            var listItems = (IEnumerable<ListItem>)actionContext.ActionArguments["items"];
            var enumerable = listItems as ListItem[] ?? listItems.ToArray();

            if (!enumerable.Any())
            {
                actionContext.Response =
                    request.CreateErrorResponse(HttpStatusCode.Forbidden, "Putting empty collection is not allowed!");
            }

            var diffCheck = new HashSet<Guid>();
            if (!enumerable.All(item => diffCheck.Add(item.Id)))
            {
                actionContext.Response =
                    request.CreateErrorResponse(HttpStatusCode.BadRequest, "GUIDs in given collection are not unique!");
            }
        }
    }
}