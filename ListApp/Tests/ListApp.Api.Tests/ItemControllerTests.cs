using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Controllers.V1;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Api.Tests.Extensions;
using NSubstitute;
using NUnit.Framework;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private static readonly Guid PostedItemGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private const string PostedItemText = "Build a monument";
        private static readonly ListItem PostedItem = new ListItem { Id = Guid.Empty, Text = PostedItemText };

        private ItemsController _itemsController;
        private IRepository _itemsRepository;
        private IRouteHelper _routeHelper;
        private IGuidGenerator _guidGenerator;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository>();
            _routeHelper = Substitute.For<IRouteHelper>();
            _guidGenerator = Substitute.For<IGuidGenerator>();

            _itemsController =
                new ItemsController(_itemsRepository, _routeHelper, _guidGenerator)
                {
                    Configuration = new HttpConfiguration(),
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
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a Coffey"},
                new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world"}
            };
            _itemsRepository.GetAllAsync().Returns(expectedItems);

            var receivedResponse = await _itemsController.GetAsync();
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out IEnumerable<ListItem> responseItems);

            _itemsRepository.Received(1).GetAllAsync();
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
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
            _itemsRepository.GetAsync(Arg.Any<Guid>()).Returns(expectedItem);

            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).GetAsync(Arg.Any<Guid>());
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Post_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocationAndCallsRepoAddAsyncOnce()
        {
            var expectedLocation = Guid.Empty.ToString();
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = PostedItemGuid,
                Text = "Stretch correctly"
            };
            _itemsRepository.AddAsync(Arg.Any<ListItem>()).Returns(expectedItem);
            _routeHelper.GetItemUrl(Arg.Any<Guid>()).Returns(expectedLocation);
            _guidGenerator.GenerateGuid().Returns(PostedItemGuid);

            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).AddAsync(Arg.Any<ListItem>());
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocationAndCallsRepoAddAndDeleteOnce()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = PostedItemGuid,
                Text = "UpdatedItem"
            };
            _itemsRepository.UpdateAsync(Arg.Any<Guid>(), Arg.Any<ListItem>()).Returns(expectedItem);

            var receivedResponse = await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).UpdateAsync(PostedItemGuid, PostedItem);
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Delete_WithAnyId_ResponseIsOfCorrectTypeAndReturnsDefaultItemAndCallsRepoDeleteAsyncOnce()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = PostedItemGuid,
                Text = "Stretch correctly"
            };
            _itemsRepository.DeleteAsync(Arg.Any<Guid>()).Returns(expectedItem);

            var receivedResponse = await _itemsController.DeleteAsync(PostedItemGuid);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).DeleteAsync(PostedItemGuid);
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }
    }
}
