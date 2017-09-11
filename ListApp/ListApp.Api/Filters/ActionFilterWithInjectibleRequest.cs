using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ListApp.Api.Filters
{
    public abstract class ActionFilterWithInjectibleRequest : ActionFilterAttribute
    {
        protected HttpRequestMessage TestRequestMessage { get; }

        protected ActionFilterWithInjectibleRequest() { }

        protected ActionFilterWithInjectibleRequest(HttpRequestMessage testMessage)
        {
            TestRequestMessage = testMessage;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            DoValidation(actionContext, TestRequestMessage ?? actionContext.Request);
        }

        protected abstract void DoValidation(HttpActionContext actionContext, HttpRequestMessage request);
    }
}