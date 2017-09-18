using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Results;
using ListApp.Api.Controllers.V1;
using ListApp.Api.Models;
using ListApp.Api.Tests.Extensions;
using ListApp.Api.Utils;
using NSubstitute;
using NUnit.Framework;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private static readonly Guid PostedItemGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private const string PostedItemText = "Build a monument";
        private static readonly ListItem PostedItem = new ListItem { Id = PostedItemGuid, Text = PostedItemText };

        private ItemsController _itemsController;

        [SetUp]
        public void SetUp()
        {
            _itemsController = new ItemsController();
            _itemsController.Configuration = Substitute.For<HttpConfiguration>();
            _itemsController.Request = Substitute.For<HttpRequestMessage>();
        }

        [Test]
        public async Task Get_NoId_ResponseIsOfCorrectTypeAndReturnsDefaultItems()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItems = new []
            {
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Text = "Stretch correctly"},
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a coffey"},
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world"}
            };

            var receivedResponse = await _itemsController.GetAsync();
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.IsTrue(responseMessage.TryGetContentValue(out IEnumerable<ListItem> responseItems));
            Assert.That(responseItems, Is.EqualTo(expectedItems).UsingListItemComparer());
        }

        [Test]
        public async Task Get_WithAnyId_ResponseIsOfCorrectTypeAndReturnsFirtsItem()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                Text = "Stretch correctly"
            };

            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Post_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocation()
        {
            var expectedLocation = _itemsController.Url.Request.RequestUri + "/00000000-0000-0000-0000-000000000003";
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Text = "Create another ListItem item!"
            };

            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocation()
        {
            var expectedLocation = _itemsController.Url.Request.RequestUri + "/00000000-0000-0000-0000-000000000003";
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Text = "Create another ListItem item!"
            };

            var receivedResponse = await _itemsController.PutAsync(Guid.NewGuid(), PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Delete_WithAnyId_ReturnsNoContent()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.NoContent;

            var receivedResponse = await _itemsController.DeleteAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
        }
    }
}
