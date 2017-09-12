using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using ListApp.Api.Controllers.V1;
using ListApp.Api.Interfaces;
using ListApp.Api.Models;
using ListApp.Api.Tests.Utils;
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
        private IRepository<Guid, ListItem> _itemsRepository;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository<Guid, ListItem>>();
            _itemsController = new ItemsController(_itemsRepository);
        
            _itemsRepository.GetAllAsync().Returns(Constants.MockListItems);
            _itemsRepository.GetAsync(Arg.Any<Guid>()).Returns(Constants.MockListItems.ElementAt(0));
        }

        #region GET tests

        [Test]
        public async Task Get_NoId_CallsRepoGetAllAsyncMethodOnce()
        {
            var receivedResponse = await _itemsController.GetAsync();
            await _itemsRepository.Received(1).GetAllAsync();
        }

        [Test]
        public async Task Get_NoId_ResponseIsOfCorrectType()
        {
            var receivedResponse = await _itemsController.GetAsync();
            Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<ListItem>>>(receivedResponse);
        }

        [Test]
        public async Task Get_NoId_ReturnsAllDefaultItems()
        {
            var receivedResponse = await _itemsController.GetAsync();
        
            var receivedItems = ((OkNegotiatedContentResult<IEnumerable<ListItem>>)receivedResponse).Content;
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Get_WithAnyId_CallsRepoGetAsyncMethodOnceWithCorrectId()
        {
            var guid = Guid.NewGuid();
            await _itemsController.GetAsync(guid);
            await _itemsRepository.Received(1).GetAsync(Arg.Any<Guid>());
        }

        [Test]
        public async Task Get_WithAnyId_ResponseIsOfCorrectType()
        {
            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);
        }


        [Test]
        public async Task Get_WithAnyId_ReturnsFirstItem()
        {
            var receivedResponse = await _itemsController.GetAsync(Guid.NewGuid());
        
            var receivedItems = ((OkNegotiatedContentResult<ListItem>)receivedResponse).Content;
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems.ElementAt(0)).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region POST tests

        [Test]
        public async Task Post_ValidItem_CallsRepoAddAsyncOnce()
        {
            await _itemsController.PostAsync(PostedItem);
            await _itemsRepository.Received(1).AddAsync(PostedItemGuid, PostedItem);
        }

        [Test]
        public async Task Post_ValidItem_ResponseIsOfCorrectType()
        {
            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);
        }


        [Test]
        public async Task Post_ValidItem_ReturnsPostedItemAndCorrectLocation()
        {
            var receivedResponse = await _itemsController.PostAsync(PostedItem);
            var castResponse = (CreatedNegotiatedContentResult<ListItem>)receivedResponse;

            var receivedItem = castResponse.Content;
            var receivedLocation = castResponse.Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{PostedItemGuid}"));
        }

        #endregion

        #region PUT tests

        [Test]
        public async Task Put_ValidItem_CallsRepoAddAndRepoDeleteAsyncOnce()
        {
            await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            await _itemsRepository.Received(1).DeleteAsync(PostedItemGuid);
            await _itemsRepository.Received(1).AddAsync(PostedItemGuid, PostedItem);
        }

        [Test]
        public async Task Put_ValidItem_ResponseIsOfCorrectType()
        {
            var receivedResponse = await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);
        }

        [Test]
        public async Task Put_ValidItem_ReturnsPutItem()
        {
            var receivedResponse = await _itemsController.PutAsync(PostedItemGuid, PostedItem);
            var castResponse = (CreatedNegotiatedContentResult<ListItem>)receivedResponse;

            var receivedItem = castResponse.Content;
            var receivedLocation = castResponse.Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{PostedItemGuid}"));
        }

        #endregion

        #region DELETE tests

        [Test]
        public async Task Put_ValidItem_CallsRepoDeleteAsyncOnce()
        {
            await _itemsController.DeleteAsync(PostedItemGuid);
            await _itemsRepository.Received(1).DeleteAsync(PostedItemGuid);
        }

        [Test]
        public async Task Delete_WithAnyId_ReturnsNoContent()
        {
            var receivedResponse = await _itemsController.DeleteAsync(Guid.NewGuid());
            Assert.IsInstanceOf<StatusCodeResult>(receivedResponse);

            Assert.AreEqual(HttpStatusCode.NoContent, ((StatusCodeResult) receivedResponse).StatusCode);
        }

        #endregion
    }
}
