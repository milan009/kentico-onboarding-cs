using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using ListApp.Api.Controllers.V1;
using ListApp.Api.Models;
using ListApp.Api.Tests.Extensions;
using ListApp.Api.Utils;
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
        }

        #region GET tests

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
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems).UsingListItemComparer());
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
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems[0]).UsingListItemComparer());
        }

        #endregion

        #region POST tests

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

            Assert.That(receivedItem, Is.EqualTo(PostedItem).UsingListItemComparer());
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{PostedItemGuid}"));
        }

        #endregion

        #region PUT tests

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

            Assert.That(receivedItem, Is.EqualTo(PostedItem).UsingListItemComparer());
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{PostedItemGuid}"));
        }

        #endregion

        #region DELETE tests

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
