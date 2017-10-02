﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using ListApp.Api.Controllers.V1;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Api.Tests.Extensions;
using ListApp.Utils;
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
        private IRepository _itemsRepository;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository>();
            _itemsRepository.GetAllAsync().Returns(Constants.MockListItems);
            _itemsRepository.GetAsync(Arg.Any<Guid>()).Returns(Constants.MockListItems.ElementAt(0));

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("itemsBaseRoute", "api/v{version}/items", new {version = 1});
            config.Routes.MapHttpRoute("itemsPutRoute", "api/v{version}/items/{id}", new {version = 1});

            _itemsController =
                new ItemsController(_itemsRepository)
                {
                    Configuration = config,
                    Request = Substitute.For<HttpRequestMessage>()
                };
        }

        [TearDown]
        public void TearDown()
        {
            _itemsController.Dispose();
        }
        
        [Test]
        public async Task Get_NoId_ResponseIsOfCorrectTypeAndReturnsDefaultItemsAndCallsRepoGetAsyncMethodOnce()
        {
     
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItems = new []
            {
                new ListItem {Id = Guid.Empty, Text = "Stretch correctly"},
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a coffey"},
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world"}
            };

            var receivedResponse = await _itemsController.GetAsync();
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).GetAllAsync());
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.IsTrue(responseMessage.TryGetContentValue(out IEnumerable<ListItem> responseItems));
            Assert.That(responseItems, Is.EqualTo(expectedItems).UsingListItemComparer());
        }

        [Test]
        public async Task Get_WithAnyId_ResponseIsOfCorrectTypeAndReturnsFirtsItemAndCallsRepoGetAsyncMethodOnceWithCorrectId()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = "Stretch correctly"
            };

            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).GetAsync(Arg.Any<Guid>()));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Post_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocationAndCallsRepoAddAsyncOnce()
        {
            var expectedLocation = $"/api/v1/items/{PostedItemGuid}";
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = PostedItemGuid,
                Text = "Create another ListItem item!"
            };

            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).AddAsync(PostedItemGuid, PostedItem));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocationAndCallsRepoAddAndDeleteOnce()
        {
            var expectedLocation = $"/api/v1/items/{PostedItemGuid}";
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = PostedItemGuid,
                Text = "Create another ListItem item!"
            };

            var receivedResponse = await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).UpdateAsync(PostedItemGuid, PostedItem));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Delete_WithAnyId_ReturnsNoContentAndCallsRepoDeleteAsyncOnce()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.NoContent;

            var receivedResponse = await _itemsController.DeleteAsync(PostedItemGuid);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).DeleteAsync(PostedItemGuid));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
        }
    }
}