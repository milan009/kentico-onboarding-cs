using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
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

        private ItemsController _theController;

        [SetUp]
        public void SetUp()
        {
            ItemsController.InitializeItems();
            _theController = new ItemsController(GuidCreator);
        }

        #region GET tests
        
        [Test]
        public async Task Get_ReturnsAllDefaultItems()
        {
            var receivedItems = await _theController.GetItems();
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems).Using(new ListItemEqualityComparer()));
        }

        // It is not possible to send a invalid GUID object, but it can easily happen when using the API
        // The server/client has to be used to test this.
        // The argument validity check is handled by the action filter
        [Test]
        public async Task Get_Returns400OnInvalidGuidFormat()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse = await client.GetAsync("http://localhost:57187/api/v1/items/1");

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
            }
        }

        [Test]
        public async Task Get_Returns404OnNonExistingItem()
        {
            var receivedResponse = await _theController.GetItem(Guid.Parse("00000000-0000-0000-0000-000000000004"));
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Get_ReturnsSpecificExistingItem()
        {
            var expectedItem = Constants.MockListItems.ElementAt(1);

            var receivedResponse = await _theController.GetItem(Guid.Parse("00000000-0000-0000-0000-000000000001"));
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region POST tests

        // The server/client has to be used to test this.
        // The null argument check is handled by the action filter
        [Test]
        public async Task Post_Returns400OnNullText()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse =
                    await client.PostAsJsonAsync<string>(new Uri("http://localhost:57187/api/v1/items"), null);

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
            }
        }

        [Test]
        public async Task Post_ReturnsPostedItem()
        {
            var receivedResponse = await _theController.PostItem(PostedItemText);

            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);
            var typedResponse = (CreatedNegotiatedContentResult<ListItem>)receivedResponse;

            var receivedItem = (typedResponse).Content;
            var receivedLocation = (typedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }

        [Test]
        public async Task Post_AddsPostedItemCorrectly()
        {
            var receivedPostResponse = await _theController.PostItem(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPostResponse);

            var receivedGetResponse = await _theController.GetItem(TheGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region PUT tests

        [Test]
        // The server/client has to be used to test this.
        // The null argument check is handled by the action filter
        public async Task Put_Returns400OnNullListItem()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse = await client.PutAsJsonAsync<ListItem>(
                    new Uri("http://localhost:57187/api/v1/items/00000000-0000-0000-0000-000000000004"), null);

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
            }
        }

        // The server/client has to be used to test this.
        // The ID consistency check is handled by the action filter
        [Test]
        public async Task Put_Returns400OnInconsistentIDs()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse =
                    await client.PutAsJsonAsync<ListItem>(new Uri("http://localhost:57187/api/v1/items/00000000-0000-0000-0000-000000000007"), PostedItem);

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
            }
        }

        [Test]
        public async Task Put_ReturnsPutItem()
        {
            var receivedResponse = await _theController.PutItem(TheGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Content;
            var receivedLocation = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }

        [Test]
        public async Task Put_AddsNewItemCorrectly()
        {
            var receivedPutResponse = await _theController.PutItem(TheGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);

            var receivedGetResponse = await _theController.GetItem(TheGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Put_ReplacesExistingItemCorrectly()
        {
            var conflictingGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var conflictingListItem = new ListItem
            {
                Id = conflictingGuid,
                Text = "Take a break"
            };

            var receivedPutResponse = await _theController.PutItem(conflictingGuid, conflictingListItem);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedPutResponse);

            var receivedGetResponse = await _theController.GetItem(conflictingGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(conflictingListItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Put_ReplacesCollectionCorrectly()
        {
            var postedCollection = new List<ListItem>();
            postedCollection.Add(PostedItem);

            var receivedResponse = await _theController.PutItemsCollection(postedCollection);
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<ListItem>>>(receivedResponse);

            var receivedCollection = ((OkNegotiatedContentResult<List<ListItem>>) receivedResponse).Content;
            Assert.That(receivedCollection, Is.EqualTo(postedCollection).AsCollection.Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region DELETE tests

        [Test]
        public async Task Delete_Returns404OnNonExistingItem()
        {
            var receivedResponse = await _theController.DeleteItem(Guid.Parse("00000000-0000-0000-0000-000000000004"));
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Delete_Returns200OnSuccesfullDelete()
        {
            var receivedResponse = await _theController.DeleteItem(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            Assert.IsInstanceOf<OkResult>(receivedResponse);
        }

        [Test]
        public async Task Delete_DeletesItemCorrectly()
        {
            var expectedItems = Constants.MockListItems.Take(2).ToList();

            var receivedDeleteResponse = await _theController.DeleteItem(Guid.Parse("00000000-0000-0000-0000-000000000002"));
            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);

            var receivedItems = await _theController.GetItems();
            Assert.That(receivedItems, Is.EqualTo(expectedItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Delete_Returns404OnNonExistingIDInCollection()
        {
            var receivedDeleteResponse = await _theController.DeleteItems(new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000005")
            });
            Assert.AreEqual(((NegotiatedContentResult<string>)receivedDeleteResponse).StatusCode, HttpStatusCode.NotFound);
        }

        [Test]
        public async Task Delete_DeletesByIDCollectionCorrectly()
        {
            var expectedItems = Constants.MockListItems.Take(1).ToList();

            var receivedDeleteResponse = await _theController.DeleteItems(new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000001")
            });
            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);

            var receivedItems = await _theController.GetItems();
            Assert.That(receivedItems, Is.EqualTo(expectedItems).Using(new ListItemEqualityComparer()));
        }

        #endregion

    }
}
