using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Results;
using NUnit.Framework;
using ListApp.Api.Controllers.V1;
using ListApp.Api.Models;
using ListApp.Api.Utils;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private static readonly Guid PostedItemGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private const string PostedItemText = "Build a monument";
        private static readonly ListItem PostedItem = new ListItem { Id = PostedItemGuid, Text = PostedItemText };

        private static readonly Func<Guid> GuidCreator = () => PostedItemGuid;

        private ItemsController _itemsController;

        [SetUp]
        public void SetUp()
        {
            _itemsController = new ItemsController(GuidCreator);
        }

        #region GET tests
        
        [Test]
        public async Task Get_ReturnsAllDefaultItems()
        {
            var receivedResponse = await _itemsController.GetItemsAsync();
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<ListItem>>>(receivedResponse);

            var receivedItems = ((OkNegotiatedContentResult<List<ListItem>>)receivedResponse).Content;
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Get_NonExistingItem_ReturnsNotFound()
        {
            var receivedResponse = await _itemsController.GetItemAsync(Constants.NonExistingItemGuid);
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Get_ExistingItem_ReturnsCorrectly()
        {
            var expectedItem = Constants.MockListItems.ElementAt(1);

            var receivedResponse = await _itemsController.GetItemAsync(Constants.Guid2);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region POST tests

        [Test]
        public async Task Post_ValidText_ReturnsPostedItem()
        {
            var receivedResponse = await _itemsController.PostItemAsync(PostedItemText);

            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);
            var typedResponse = (CreatedNegotiatedContentResult<ListItem>)receivedResponse;

            var receivedItem = (typedResponse).Content;
            var receivedLocation = (typedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{PostedItemGuid}"));
        }

        [Test]
        public async Task Post_ValidText_AddsPostedItemCorrectly()
        {
            var receivedPostResponse = await _itemsController.PostItemAsync(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPostResponse);

            var receivedGetResponse = await _itemsController.GetItemAsync(PostedItemGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region PUT tests
        [Test]
        public async Task Put_ValidItem_ReturnsPutItem()
        {
            var receivedResponse = await _itemsController.PutItemAsync(PostedItemGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Content;
            var receivedLocation = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{PostedItemGuid}"));
        }

        [Test]
        public async Task Put_ValidItem_AddsNewItemCorrectly()
        {
            var receivedPutResponse = await _itemsController.PutItemAsync(PostedItemGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);

            var receivedGetResponse = await _itemsController.GetItemAsync(PostedItemGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Put_ValidItemToExistingGuid_ReplacesExistingItemCorrectly()
        {
            var conflictingGuid = Constants.Guid2;
            var conflictingListItem = new ListItem
            {
                Id = conflictingGuid,
                Text = "Take a break"
            };

            var receivedPutResponse = await _itemsController.PutItemAsync(conflictingGuid, conflictingListItem);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedPutResponse);

            var receivedGetResponse = await _itemsController.GetItemAsync(conflictingGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(conflictingListItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Put_ValidCollection_ReturnsPutCollection()
        {
            var postedCollection = new List<ListItem>{PostedItem};

            var receivedResponse = await _itemsController.PutItemsCollectionAsync(postedCollection);
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<ListItem>>>(receivedResponse);

            var receivedCollection = ((OkNegotiatedContentResult<List<ListItem>>) receivedResponse).Content;
            Assert.That(receivedCollection, Is.EqualTo(postedCollection).AsCollection.Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region DELETE tests

        [Test]
        public async Task Delete_NonExistingItem_ReturnsNotFound()
        {
            var receivedResponse = await _itemsController.DeleteItemAsync(Constants.NonExistingItemGuid);
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Delete_ExistingItem_ReturnsOk()
        {
            var receivedResponse = await _itemsController.DeleteItemAsync(Constants.Guid3);
            Assert.IsInstanceOf<OkResult>(receivedResponse);
        }

        [Test]
        public async Task Delete_ExistingItem_DeletesItemCorrectly()
        {
            var expectedItems = Constants.MockListItems.Take(2).ToList();

            var receivedDeleteResponse = await _itemsController.DeleteItemAsync(Constants.Guid3);
            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);

            var receivedResponse = await _itemsController.GetItemsAsync();
            var receivedItems = ((OkNegotiatedContentResult<List<ListItem>>) receivedResponse).Content;
            Assert.That(receivedItems, Is.EqualTo(expectedItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Delete_NonExistingIdInCollection_ReturnsNotFound()
        {
            var receivedDeleteResponse = await _itemsController.DeleteItemsCollectionAsync(new List<Guid>
            {
                Constants.Guid2,
                Constants.NonExistingItemGuid
            });
            Assert.AreEqual(((NegotiatedContentResult<string>)receivedDeleteResponse).StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_ValidIdCollection_DeletesCorrectly()
        {
            var expectedItems = Constants.MockListItems.Take(1).ToList();

            var receivedDeleteResponse = await _itemsController.DeleteItemsCollectionAsync(new List<Guid>
            {
                Constants.Guid2,
                Constants.Guid3
            });
            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);

            var receivedResponse = await _itemsController.GetItemsAsync();
            var receivedItems = ((OkNegotiatedContentResult<List<ListItem>>)receivedResponse).Content;
            Assert.That(receivedItems, Is.EqualTo(expectedItems).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region PATCH tests

        [Test]
        public async Task Patch_NonExistingItem_ReturnsNotFound()
        {
            var patch = new JsonPatch.JsonPatchDocument<ListItem>();
            patch.Replace("/Text", "Buy some aubergine!");

            var receivedResponse = await _itemsController.PatchItemAsync(Constants.NonExistingItemGuid, patch);
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Patch_ValidItem_UpdatesListItemCorrectly()
        {
            var newText = "Take a bath";
            var expectedItem = new ListItem
            {
                Id = Constants.Guid1,
                Text = newText
            };

            var patch = new JsonPatch.JsonPatchDocument<ListItem>();
            patch.Replace("/Text", newText);

            var receivedResponse = await _itemsController.PatchItemAsync(Constants.Guid1, patch);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((OkNegotiatedContentResult<ListItem>) receivedResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }
        #endregion
    }
}
