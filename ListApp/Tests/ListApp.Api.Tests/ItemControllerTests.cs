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
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private ItemsController _itemsController;
        private IRepository _itemsRepository;
        private IRouteHelper _routeHelper;
        private IInsertItemService _insertItemService;
        private IUpdateItemService _updateItemService;
        private IDeleteItemService _deleteItemService;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository>();
            _routeHelper = Substitute.For<IRouteHelper>();
            _insertItemService = Substitute.For<IInsertItemService>();
            _updateItemService = Substitute.For<IUpdateItemService>();
            _deleteItemService = Substitute.For<IDeleteItemService>();

            _itemsController =
                new ItemsController(_itemsRepository, _routeHelper, _insertItemService, _deleteItemService, _updateItemService)
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

            _itemsRepository.GetAllAsync()
                .ReturnsForAnyArgs(expectedItems);

            var receivedResponse = await _itemsController.GetAsync();
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out IEnumerable<ListItem> responseItems);

            await _itemsRepository.Received(1).GetAllAsync();
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItems, Is.EqualTo(expectedItems).UsingListItemComparer());
        }

        [Test]
        public async Task Get_ExistingID_ReturnsOkResponse()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"),
                Text = "Stretch correctly"
            };

            _itemsRepository.GetAsync(Arg.Any<Guid>())
                .Returns(expectedItem);

            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            await _itemsRepository.Received(1).GetAsync(Arg.Any<Guid>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Get_NonExistingID_ReturnsNotFound()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.NotFound;

            _itemsRepository.GetAsync(Arg.Any<Guid>())
                .ReturnsNull();

            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            await _itemsRepository.Received(1).GetAsync(Arg.Any<Guid>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Post_NullItem_ReturnsBadRequest()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.BadRequest;

            var receivedResponse = await _itemsController.PostAsync(null);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            await _insertItemService.Received(0).InsertItemAsync(Arg.Any<ListItem>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Post_ItemWithGivenId_ReturnsBadRequest()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.BadRequest;
            var postedItem = new ListItem{Id = Guid.NewGuid(), Text = "An invalid post item"};

            var receivedResponse = await _itemsController.PostAsync(postedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            await _insertItemService.Received(0).InsertItemAsync(Arg.Any<ListItem>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Post_ValidItem_ReturnsCreatedResponse()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var expectedLocation = "BFBF8A62-FD82-42D4-A86B-324704BE161E";
            var postedItem = new ListItem {Id = Guid.Empty, Text = "Stretch correctly"};
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"),
                Text = "Stretch correctly"
            };

            _insertItemService.InsertItemAsync(Arg.Any<ListItem>())
                .Returns(expectedItem);
            _routeHelper.GetItemUrl(Arg.Any<Guid>())
                .Returns(expectedLocation);

            var receivedResponse = await _itemsController.PostAsync(postedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            await _insertItemService.Received(1).InsertItemAsync(postedItem);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseMessage.Headers.Location.ToString(), Is.EqualTo(expectedLocation));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_NullItem_ReturnsBadRequest()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.BadRequest;

            var receivedResponse = await _itemsController.PutAsync(Guid.NewGuid(), null);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            await _updateItemService.Received(0).CheckIfItemExistsAsync(Arg.Any<ListItem>());
            await _updateItemService.Received(0).UpdateItemAsync(Arg.Any<ListItem>(), Arg.Any<ListItem>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Put_EmptyGuid_ReturnsBadRequest()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.BadRequest;
            var putItem = new ListItem { Id = Guid.Empty, Text = "Stretch correctly" };

            var receivedResponse = await _itemsController.PutAsync(Guid.Empty, putItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            await _updateItemService.Received(0).CheckIfItemExistsAsync(Arg.Any<ListItem>());
            await _updateItemService.Received(0).UpdateItemAsync(Arg.Any<ListItem>(), Arg.Any<ListItem>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Put_InconsistentIDs_ReturnsBadRequest()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.BadRequest;
            var putItem = new ListItem
            {
                Id = Guid.Parse("6B2FC8DD-E372-458B-A0A6-654EA545BFB9"),
                Text = "Stretch correctly"
            };

            var receivedResponse = await _itemsController.PutAsync(Guid.Parse("00000000-E372-458B-A0A6-654EA545BFB9"), putItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);

            await _updateItemService.Received(0).CheckIfItemExistsAsync(Arg.Any<ListItem>());
            await _updateItemService.Received(0).UpdateItemAsync(Arg.Any<ListItem>(), Arg.Any<ListItem>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Put_NonExistingItem_ReturnsCreated()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.Created;
            var itemGuid = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E");
            var expectedLocation = itemGuid.ToString();
            var expectedItem = new ListItem { Id = itemGuid, Text = "UpdatedItem" };

            _updateItemService.CheckIfItemExistsAsync(Arg.Any<ListItem>())
                .Returns(OperationResult.Failed);
            _insertItemService.InsertItemAsync(Arg.Any<ListItem>())
                .Returns(expectedItem);
            _routeHelper.GetItemUrl(Arg.Any<Guid>())
                .Returns(expectedLocation);

            var receivedResponse = await _itemsController.PutAsync(expectedItem.Id, expectedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            await _updateItemService.Received(1).CheckIfItemExistsAsync(expectedItem);
            await _updateItemService.Received(0).UpdateItemAsync(Arg.Any<ListItem>(), Arg.Any<ListItem>());
            await _insertItemService.Received(1).InsertItemAsync(expectedItem);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseMessage.Headers.Location.ToString(), Is.EqualTo(expectedLocation));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Put_ExistingItem_ReturnsOk()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var itemGuid = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E");
            var expectedItem = new ListItem { Id = itemGuid, Text = "UpdatedItem" };

            _updateItemService.CheckIfItemExistsAsync(Arg.Any<ListItem>())
                .Returns(OperationResult.CreateSuccessfulResult(expectedItem));
            _updateItemService.UpdateItemAsync(Arg.Any<ListItem>(), expectedItem)
                .Returns(OperationResult.CreateSuccessfulResult(expectedItem));

            var receivedResponse = await _itemsController.PutAsync(expectedItem.Id, expectedItem);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            await _updateItemService.Received(1).CheckIfItemExistsAsync(expectedItem);
            await _updateItemService.Received(1).UpdateItemAsync(Arg.Any<ListItem>(), expectedItem);
            await _insertItemService.Received(0).InsertItemAsync(Arg.Any<ListItem>());
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }

        [Test]
        public async Task Delete_NonExistingId_ReturnNotFound()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.NotFound;
            var deletedGuid = Guid.NewGuid();

            _deleteItemService.DeleteItemAsync(Arg.Any<Guid>())
                .Returns(OperationResult.Failed);

            var receivedResponse = await _itemsController.DeleteAsync(deletedGuid);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            await _deleteItemService.Received(1).DeleteItemAsync(deletedGuid);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
        }

        [Test]
        public async Task Delete_ExistingId_ReturnsOkResponse()
        {
            const HttpStatusCode expectedResponseCode = HttpStatusCode.OK;
            var deletedGuid = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E");
            var expectedItem = new ListItem
            {
                Id = deletedGuid,
                Text = "Stretch correctly"
            };

            _deleteItemService.DeleteItemAsync(Arg.Any<Guid>())
                .Returns(OperationResult.CreateSuccessfulResult(expectedItem));

            var receivedResponse = await _itemsController.DeleteAsync(expectedItem.Id);
            var responseMessage = await receivedResponse.ExecuteAsync(CancellationToken.None);
            responseMessage.TryGetContentValue(out ListItem responseItem);

            await _deleteItemService.Received(1).DeleteItemAsync(deletedGuid);
            Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedResponseCode));
            Assert.That(responseItem, Is.EqualTo(expectedItem).UsingListItemComparer());
        }
    }
}
