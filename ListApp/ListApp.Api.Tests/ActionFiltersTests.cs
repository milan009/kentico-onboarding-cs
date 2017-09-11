using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using ListApp.Api.Filters;
using ListApp.Api.Models;
using NUnit.Framework;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ModelValidationFilterTests
    {
        [Test]
        public void OnActionExecuting_ValidModel_NoResponse()
        {
            var actionContext = new HttpActionContext();
            actionContext.ModelState.Clear();
            var actionFilter = new ModelValidationActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.IsNull(actionContext.Response);
        }

        [Test]
        public void OnActionExecuting_InvalidModel_BadRequestResponse()
        {
            var actionContext = new HttpActionContext();
            actionContext.ModelState.AddModelError("id", new ArgumentException());
            var actionFilter = new ModelValidationActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.AreEqual(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
        }
    }

    [TestFixture]
    public class NullArgumentFilterTests
    {
        [Test]
        public void OnActionExecuting_ValidArgument_NoResponse()
        {
            var actionContext = new HttpActionContext();
            actionContext.ActionArguments.Add("id", "751A9F68-E4B0-4D61-B3E8-4E126D2A5B2B");
            var actionFilter = new NullArgumentActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.IsNull(actionContext.Response);
        }

        [Test]
        public void OnActionExecuting_NullArgument_BadRequestResponse()
        {
            var actionContext = new HttpActionContext();
            actionContext.ActionArguments.Add("id", null);
            var actionFilter = new NullArgumentActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.AreEqual(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
        }
    }

    [TestFixture]
    public class PutGuidConsistencyFilterTests
    {
        [Test]
        public void OnActionExecuting_MatchingGuids_NoResponse()
        {
            var actionContext = new HttpActionContext();
            var theGuid = Guid.Parse("751A9F68-E4B0-4D61-B3E8-4E126D2A5B2B");
            actionContext.ActionArguments.Add("id", theGuid);
            actionContext.ActionArguments.Add("newItem", new ListItem
            {
                Id = theGuid,
                Text = "Some text"
            });

            var actionFilter = new PutGuidConsistencyActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.IsNull(actionContext.Response);
        }

        [Test]
        public void OnActionExecuting_DifferentGuids_BadRequestResponse()
        {
            var actionContext = new HttpActionContext();
            var guid1 = Guid.Parse("751A9F68-0000-4D61-B3E8-4E126D2A5B2B");
            var guid2 = Guid.Parse("751A9F68-0000-0000-B3E8-4E126D2A5B2B");

            actionContext.ActionArguments.Add("id", guid1);
            actionContext.ActionArguments.Add("newItem", new ListItem
            {
                Id = guid2,
                Text = "Some text"
            });

            var actionFilter = new PutGuidConsistencyActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.AreEqual(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
        }
    }

    [TestFixture]
    public class PutCollectionFilterTests
    {
        [Test]
        public void OnActionExecutingTests_ValidCollection_NoResponse()
        {
            var actionContext = new HttpActionContext();
            var theCollection = new List<ListItem>
            {
                new ListItem {Id = Guid.Parse("751A9F68-0000-4D61-B3E8-4E126D2A5B2B"), Text = "Some text"},
                new ListItem {Id = Guid.Parse("751A9F68-0000-0000-B3E8-4E126D2A5B2B"), Text = "Some other text"},
                new ListItem {Id = Guid.Parse("751A9F68-0000-0000-0000-4E126D2A5B2B"), Text = "Some different text"}
            };
            actionContext.ActionArguments.Add("items", theCollection);

            var actionFilter = new PutCollectionActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.IsNull(actionContext.Response);
        }

        [Test]
        public void OnActionExecuting_ConflictingGUIDs_BadRequestResponse()
        {
            var actionContext = new HttpActionContext();
            var theCollection = new List<ListItem>
            {
                new ListItem {Id = Guid.Parse("751A9F68-0000-4D61-B3E8-4E126D2A5B2B"), Text = "Some text"},
                new ListItem {Id = Guid.Parse("751A9F68-0000-0000-0000-4E126D2A5B2B"), Text = "Some other text"},
                new ListItem {Id = Guid.Parse("751A9F68-0000-0000-0000-4E126D2A5B2B"), Text = "Some different text but same guid"}
            };
            actionContext.ActionArguments.Add("items", theCollection);

            var actionFilter = new PutCollectionActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.AreEqual(HttpStatusCode.BadRequest, actionContext.Response.StatusCode);
        }

        [Test]
        public void OnActionExecuting_EmptyCollection_ForbiddenResponse()
        {
            var actionContext = new HttpActionContext();
            var theCollection = new List<ListItem>();
            actionContext.ActionArguments.Add("items", theCollection);

            var actionFilter = new PutCollectionActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.AreEqual(HttpStatusCode.Forbidden, actionContext.Response.StatusCode);
        }
    }

    [TestFixture]
    public class PatchSingleResourceFilterTests
    {
        [Test]
        public void OnActionExecuting_ValidPatchRequest_NoResponse()
        {
            var actionContext = new HttpActionContext();
            var patch = new JsonPatch.JsonPatchDocument<ListItem>();
            patch.Replace("/Text", "Replaced text");

            actionContext.ActionArguments.Add("patch", patch);

            var actionFilter = new PatchSingleResourceActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.IsNull(actionContext.Response);
        }

        [Test]
        public void OnActionExecuting_ForbiddenOperation_ForbiddenResponse()
        {
            var actionContext = new HttpActionContext();
            var patch = new JsonPatch.JsonPatchDocument<ListItem>();
            patch.Remove("/Text");

            actionContext.ActionArguments.Add("patch", patch);

            var actionFilter = new PatchSingleResourceActionFilter(new HttpRequestMessage());

            actionFilter.OnActionExecuting(actionContext);

            Assert.AreEqual(HttpStatusCode.Forbidden, actionContext.Response.StatusCode);
        }
    }
}
