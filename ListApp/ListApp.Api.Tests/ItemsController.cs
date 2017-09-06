using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using JsonPatch;
using NUnit.Framework;
using ListApp.Api.Controllers.V1;
using ListApp.Api.Repositories;
using ListApp.Api.Utils;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ListApp.Api.Models;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private static readonly Guid TheGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private const string PostedItemText = "Build a monument";
        private static readonly ListItem PostedItem = new ListItem { Id = TheGuid, Text = PostedItemText };

        private static readonly Func<Guid> GuidCreator = () => TheGuid;
        private IRepository<ListItem, Guid> _itemsRepository;

        private ItemsController _theController;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository<ListItem, Guid>>();
            _theController = new ItemsController(GuidCreator, _itemsRepository);
        }

        // Some of the tests require to create an instance of Http server/client,
        // as passing direct invalid method argument is not possible (GUID) OR 
        // since the validity check is done by action filters, not the controller.

        #region GET tests
        
        [Test]
        public async Task Get_ReturnsAllDefaultItems()
        {
            _itemsRepository.GetAll().Returns(Utils.Constants.MockListItems);

            var receivedItems = await _theController.GetItems();

            Assert.That(receivedItems, Is.EqualTo(Utils.Constants.MockListItems)
                .Using(new ListItemEqualityComparer()));
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
                _itemsRepository.DidNotReceive().Get(Arg.Any<Guid>());
            }
        }

        [Test]
        public async Task Get_Returns404OnNonExistingItem()
        {
            var nonExistingGuid = Guid.Parse("00000000-0000-0000-0000-000000000004");
            _itemsRepository.Get(nonExistingGuid).ReturnsNull();

            var receivedResponse = await _theController.GetItem(nonExistingGuid);

            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Get_Returns200OnExistingItem()
        {
            var targetGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            _itemsRepository.Get(targetGuid).Returns(Utils.Constants.MockListItems.ElementAt(1));

            var receivedResponse = await _theController.GetItem(targetGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);
        }

        [Test]
        public async Task Get_ReturnsSpecificExistingItem()
        {
            var targetGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var expectedItem = Utils.Constants.MockListItems.ElementAt(1);
            _itemsRepository.Get(targetGuid).Returns(Utils.Constants.MockListItems.ElementAt(1));

            var receivedResponse = await _theController.GetItem(targetGuid);
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
                _itemsRepository.DidNotReceive().Add(Arg.Any<Guid>(), Arg.Any<ListItem>());
            }
        }

        [Test]
        public async Task Post_ReturnsPostedItemAndLocation()
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
        public async Task Post_CallsAddWithCorrectGuidAndItem()
        {
            await _theController.PostItem(PostedItemText);
            
            _itemsRepository.Received().Add(TheGuid, Arg.Is<ListItem>(
                (arg) => new ListItemEqualityComparer().Equals(arg, PostedItem)));
        }

        [Test]
        public async Task Post_Returns500OnDuplicitKeyGeneration()
        {
            _itemsRepository.When(x => x.Add(TheGuid, Arg.Any<ListItem>()))
                .Do(x => throw new DuplicateKeyException(TheGuid, "Duplicate GUID generated!"));

            var receivedResponse = await _theController.PostItem(PostedItemText);
            Assert.IsInstanceOf<ExceptionResult>(receivedResponse);

            _itemsRepository.Received().Add(TheGuid, Arg.Is<ListItem>(
                (arg) => new ListItemEqualityComparer().Equals(arg, PostedItem)));
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
                _itemsRepository.DidNotReceive().Delete(Arg.Any<Guid>());
                _itemsRepository.DidNotReceive().Add(Arg.Any<Guid>(), Arg.Any<ListItem>());
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
                    await client.PutAsJsonAsync(
                        new Uri("http://localhost:57187/api/v1/items/00000000-0000-0000-0000-000000000007"), PostedItem);

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
                _itemsRepository.DidNotReceive().Delete(Arg.Any<Guid>());
                _itemsRepository.DidNotReceive().Add(Arg.Any<Guid>(), Arg.Any<ListItem>());
            }
        }

        [Test]
        public async Task Put_ReturnsPutItemAndLocation()
        {
            var receivedResponse = await _theController.PutItem(TheGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var typedResponse = (CreatedNegotiatedContentResult<ListItem>) receivedResponse;
            var receivedItem = typedResponse.Content;
            var receivedLocation = typedResponse.Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }
        
        [Test]
        public async Task Put_CallsOnlyAddWhenAddingNewItem()
        {
            _itemsRepository.GetKeys().Returns(new List<Guid>());

            var receivedPutResponse = await _theController.PutItem(TheGuid, PostedItem);

            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);
            _itemsRepository.Received().Add(TheGuid, PostedItem);
            _itemsRepository.DidNotReceive().Delete(TheGuid);
        }

        [Test]
        public async Task Put_CallsAddAndDeleteWhenReplacingExistingItem()
        {
            _itemsRepository.GetKeys().Returns(Utils.Constants.MockListItems.Select(item => item.Id));

            var conflictingGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var conflictingListItem = new ListItem
            {
                Id = conflictingGuid,
                Text = "Take a break"
            };

            var receivedPutResponse = await _theController.PutItem(conflictingGuid, conflictingListItem);

            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);
            _itemsRepository.Received().Add(conflictingGuid, conflictingListItem);
            _itemsRepository.Received().Delete(conflictingGuid);
        }

        [Test]
        public async Task Put_CollectionReturnsPutCollectionAndLocation()
        {
            var postedCollection = new List<ListItem> {PostedItem};
            _itemsRepository.GetAll().Returns(postedCollection);

            var receivedResponse = await _theController.PutItemsCollection(postedCollection);
            var typedResponse = (CreatedNegotiatedContentResult<IEnumerable<ListItem>>) receivedResponse;
            var receivedCollection = typedResponse.Content;
            var receivedLocation = typedResponse.Location;

            Assert.That(receivedCollection, Is.EqualTo(postedCollection)
                .AsCollection.Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo("/items"));
        }

        [Test]
        public async Task Put_CollectionCallsClearAddAndNotDelete()
        {
            var postedCollection = new List<ListItem> {PostedItem};

            await _theController.PutItemsCollection(postedCollection);

            _itemsRepository.Received(1).Clear();
            _itemsRepository.Received(1).Add(PostedItem.Id, PostedItem);
            _itemsRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        }

        #endregion

        #region DELETE tests

        [Test]
        public async Task Delete_Returns404OnNonExistingItem()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000004");
            _itemsRepository
                .When(x => x.Delete(theGuid))
                .Do(x => throw new KeyNotFoundException());

            var receivedResponse = await _theController.DeleteItem(theGuid);

            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
            _itemsRepository.Received().Delete(theGuid);
        }

        [Test]
        public async Task Delete_Returns200OnSuccesfullDelete()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var receivedResponse = await _theController.DeleteItem(theGuid);

            Assert.IsInstanceOf<OkResult>(receivedResponse);
            _itemsRepository.Received().Delete(theGuid);
        }

        [Test]
        public async Task Delete_CallsRepoDeleteOnExistingItem()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var receivedDeleteResponse = await _theController.DeleteItem(theGuid);

            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);
            _itemsRepository.Received().Delete(theGuid);
        }

        [Test]
        public async Task Delete_Returns404OnNonExistingIDInCollection()
        {
            _itemsRepository.GetKeys().Returns(Utils.Constants.MockListItems.Select(item => item.Id));

            var receivedDeleteResponse = await _theController.DeleteItems(new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000005")
            });

            Assert.AreEqual(((NegotiatedContentResult<string>)receivedDeleteResponse).StatusCode, HttpStatusCode.NotFound);
            _itemsRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        }

        [Test]
        public async Task Delete_CollectionCallsRepoDeleteWithCorrectIDs()
        {
            _itemsRepository.GetKeys().Returns(Utils.Constants.MockListItems.Select(item => item.Id));
            var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var receivedDeleteResponse = await _theController.DeleteItems(new List<Guid>
            {
                guid1,
                guid2               
            });

            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);
            _itemsRepository.Received().Delete(guid1);
            _itemsRepository.Received().Delete(guid2);
        }

        #endregion

        #region PATCH tests

        [Test]
        public async Task Patch_Returns404OnNotFound()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000005");
            _itemsRepository.Get(theGuid).ReturnsNull();

            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", "Buy some aubergine!");

            var receivedResponse = await _theController.PatchItem(theGuid, patch);

            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
            _itemsRepository.DidNotReceive().Add(Arg.Any<Guid>(), Arg.Any<ListItem>());
            _itemsRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        }

        [Test]
        public async Task Patch_ReturnsUpdatedListItem()
        {
            _itemsRepository.Get(Guid.Empty).Returns(Utils.Constants.MockListItems.ElementAt(0));

            var newText = "Take a bath";
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = newText
            };

            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", newText);

            var receivedResponse = await _theController.PatchItem(Guid.Empty, patch);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((OkNegotiatedContentResult<ListItem>) receivedResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Patch_CallsAddAndDeleteOnValidGuid()
        {
            _itemsRepository.Get(Guid.Empty).Returns(Utils.Constants.MockListItems.ElementAt(0));

            var newText = "Take a bath";
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = newText
            };

            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", newText);

            var receivedResponse = await _theController.PatchItem(Guid.Empty, patch);

            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);
            _itemsRepository.Received().Delete(Guid.Empty);
            _itemsRepository.Received().Add(Guid.Empty, Arg.Is<ListItem>(
                (arg) => new ListItemEqualityComparer().Equals(arg, expectedItem)));
        }

        [Test]
        public async Task Patch_Returns403OnForbiddenOperation()
        {
            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", "Buy some aubergine!");
            patch.Replace("/Id", new Guid());
            patch.Remove("/Id");

            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse =
                    await client.SendAsync(new HttpRequestMessage
                    {
                        Method = new HttpMethod("PATCH"),
                        RequestUri = new Uri("http://localhost:57187/api/v1/items/00000000-0000-0000-0000-000000000000"),
                        Content = new ObjectContent<JsonPatchDocument<ListItem>>(patch, new JsonMediaTypeFormatter())
                    });

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.Forbidden);
                _itemsRepository.DidNotReceive().Add(Arg.Any<Guid>(), Arg.Any<ListItem>());
                _itemsRepository.DidNotReceive().Delete(Arg.Any<Guid>());
            }
        }
        #endregion
    }
}
