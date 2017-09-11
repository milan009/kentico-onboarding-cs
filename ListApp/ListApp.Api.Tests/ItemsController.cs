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
        private static readonly Guid TheGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private const string PostedItemText = "Build a monument";
        private static readonly ListItem PostedItem = new ListItem { Id = TheGuid, Text = PostedItemText };
        private static readonly Func<Guid> GuidCreator = () => TheGuid;

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
            Assert.That(receivedItems, Is.EqualTo(Utils.Constants.MockListItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Get_NonExistingItem_ReturnsNotFound()
        {
            var receivedResponse = await _itemsController.GetItemAsync(Guid.Parse("00000000-0000-0000-0000-000000000004"));
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Get_ExistingItem_ReturnsCorrectly()
        {
            var expectedItem = Utils.Constants.MockListItems.ElementAt(1);

            var receivedResponse = await _itemsController.GetItemAsync(Guid.Parse("00000000-0000-0000-0000-000000000001"));
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
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }

        [Test]
        public async Task Post_ValidText_AddsPostedItemCorrectly()
        {
            var receivedPostResponse = await _itemsController.PostItemAsync(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPostResponse);

            var receivedGetResponse = await _itemsController.GetItemAsync(TheGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region PUT tests
        [Test]
        public async Task Put_ValidItem_ReturnsPutItem()
        {
            var receivedResponse = await _itemsController.PutItemAsync(TheGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Content;
            var receivedLocation = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }

        [Test]
        public async Task Put_ValidItem_AddsNewItemCorrectly()
        {
            var receivedPutResponse = await _itemsController.PutItemAsync(TheGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);

            var receivedGetResponse = await _itemsController.GetItemAsync(TheGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Put_ValidItemToExistingGuid_ReplacesExistingItemCorrectly()
        {
            var conflictingGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
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
            var postedCollection = new List<ListItem>();
            postedCollection.Add(PostedItem);

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
            var receivedResponse = await _itemsController.DeleteItemAsync(Guid.Parse("00000000-0000-0000-0000-000000000004"));
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Delete_ExistingItem_ReturnsOk()
        {
            var receivedResponse = await _itemsController.DeleteItemAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            Assert.IsInstanceOf<OkResult>(receivedResponse);
        }

        [Test]
        public async Task Delete_ExistingItem_DeletesItemCorrectly()
        {
            var expectedItems = Utils.Constants.MockListItems.Take(2).ToList();

            var receivedDeleteResponse = await _itemsController.DeleteItemAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);

            var receivedResponse = await _itemsController.GetItemsAsync();
            var receivedItems = ((OkNegotiatedContentResult<List<ListItem>>) receivedResponse).Content;
            Assert.That(receivedItems, Is.EqualTo(expectedItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Delete_NonExistingIdInCollection_ReturnsNotFound()
        {
            var receivedDeleteResponse = await _itemsController.DeleteItemsAsync(new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000005")
            });
            Assert.AreEqual(((NegotiatedContentResult<string>)receivedDeleteResponse).StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_ValidIdCollection_DeletesCorrectly()
        {
            var expectedItems = Utils.Constants.MockListItems.Take(1).ToList();

            var receivedDeleteResponse = await _itemsController.DeleteItemsAsync(new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000001")
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

            var receivedResponse = await _itemsController.PatchItemAsync(Guid.Parse("00000000-0000-0000-0000-000000000005"), patch);
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Patch_ValidItem_UpdatesListItemCorrectly()
        {
            var newText = "Take a bath";
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = newText
            };

            var patch = new JsonPatch.JsonPatchDocument<ListItem>();
            patch.Replace("/Text", newText);

            var receivedResponse = await _itemsController.PatchItemAsync(Guid.Empty, patch);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((OkNegotiatedContentResult<ListItem>) receivedResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }
        #endregion
    }
}
