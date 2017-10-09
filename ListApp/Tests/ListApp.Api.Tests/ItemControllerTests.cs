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
        private IRouteHelper _routeHelper;
        private IGuidGenerator _guidGenerator;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository>();
            _itemsRepository.GetAllAsync()
                .Returns(Constants.MockListItems);
            _itemsRepository.GetAsync(Arg.Any<Guid>())
                .Returns(Constants.MockListItems.ElementAt(0));
            _itemsRepository.AddAsync(Arg.Any<ListItem>())
                .Returns(Constants.MockListItems.ElementAt(0));
            _itemsRepository.DeleteAsync(Arg.Any<Guid>())
                .Returns(Constants.MockListItems.ElementAt(0));
            _itemsRepository.UpdateAsync(Arg.Any<Guid>(), Arg.Any<ListItem>())
                .Returns(Constants.MockListItems.ElementAt(0));

            _routeHelper = Substitute.For<IRouteHelper>();
            _routeHelper.GetItemUrl(Arg.Any<Guid>())
                .Returns(Guid.Empty.ToString());

            _guidGenerator = Substitute.For<IGuidGenerator>();
                _guidGenerator.GenerateGuid().ReturnsForAnyArgs(Guid.Empty);

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
            var expectedLocation = Guid.Empty.ToString();
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = "Stretch correctly"
            };

            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).AddAsync(PostedItem));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_ValidItem_ResponseIsOfCorrectTypeAndReturnsDefaultItemWithCorrectLocationAndCallsRepoAddAndDeleteOnce()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = "Stretch correctly"
            };

            var receivedResponse = await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).UpdateAsync(PostedItemGuid, PostedItem));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Delete_WithAnyId_ResponseIsOfCorrectTypeAndReturnsDefaultItemAndCallsRepoDeleteAsyncOnce()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = "Stretch correctly"
            };

            var receivedResponse = await _itemsController.DeleteAsync(PostedItemGuid);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            Assert.DoesNotThrowAsync(() => _itemsRepository.Received(1).DeleteAsync(PostedItemGuid));
            Assert.AreEqual(expectedResponseCode, responseMessage.StatusCode);
            Assert.IsTrue(responseMessage.TryGetContentValue(out ListItem responseItem));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }
    }
}
