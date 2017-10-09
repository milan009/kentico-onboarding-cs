using System;
using System.Collections.Generic;
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
        private IItemService _itemService;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository>();
            _routeHelper = Substitute.For<IRouteHelper>();
            _itemService = Substitute.For<IItemService>();

            _itemsController =
                new ItemsController(_itemsRepository, _routeHelper, _itemService)
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
        public async Task Get_NoId_ReturnsOkResponse()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItems = new []
            {
                new ListItem {Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"), Text = "Stretch correctly"},
                new ListItem {Id = Guid.Parse("A55578BC-57F2-4A42-BEDF-7D8C23992DBC"), Text = "Make coffee"},
                new ListItem {Id = Guid.Parse("C6F4D46F-D7B1-45DD-8C7C-265313AF77BB"), Text = "Take over the world"}
            };
            _itemsRepository.GetAllAsync().ReturnsForAnyArgs(expectedItems);

            var receivedResponse = await _itemsController.GetAsync();
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out IEnumerable<ListItem> responseItems);

            _itemsRepository.Received(1).GetAllAsync();
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItems, Is.EqualTo(expectedItems).UsingListItemComparer());
        }

        [Test]
        public async Task Get_WithAnyId_ReturnsOkResponse()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"),
                Text = "Stretch correctly"
            };
            _itemsRepository.GetAsync(Arg.Any<Guid>()).Returns(expectedItem);

            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).GetAsync(Arg.Any<Guid>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Post_ValidItem_ReturnsCreatedResponse()
        {
            var expectedLocation = Guid.Empty.ToString();
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"),
                Text = "Stretch correctly"
            };
            _itemsRepository.AddAsync(Arg.Any<ListItem>()).Returns(expectedItem);
            _routeHelper.GetItemUrl(Arg.Any<Guid>()).Returns(expectedLocation);

            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).AddAsync(PostedItem);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.AreEqual(expectedLocation, responseMessage.Headers.Location.ToString());
            Assert.That(responseMessage.Headers.Location.ToString(), Is.EqualTo(expectedLocation));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_ValidItem_ReturnsOkResponse()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"),
                Text = "UpdatedItem"
            };
            _itemsRepository.UpdateAsync(Arg.Any<ListItem>()).Returns(expectedItem);

            var receivedResponse = await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            _itemsRepository.Received(1).UpdateAsync(PostedItem);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Delete_ReturnsOkResponse()
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
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }
    }
}
