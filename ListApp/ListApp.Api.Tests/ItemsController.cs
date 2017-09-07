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

        private ItemsController _itemsController;

        [SetUp]
        public void SetUp()
        {
            _itemsRepository = Substitute.For<IRepository<ListItem, Guid>>();
            _itemsController = new ItemsController(GuidCreator, _itemsRepository);
        }

        // Some of the tests require to create an instance of Http server/client,
        // as passing direct invalid method argument is not possible (GUID) OR 
        // since the validity check is done by action filters, not the controller.

        #region GET tests
        
        [Test]
        public async Task Get_ReturnsAllDefaultItems()
        {
            _itemsRepository.GetAllAsync().Returns(Utils.Constants.MockListItems);

            var receivedResponse = await _itemsController.GetItemsAsync();
            var receivedItems = ((OkNegotiatedContentResult<IEnumerable<ListItem>>)receivedResponse).Content;

            Assert.That(receivedItems, Is.EqualTo(Utils.Constants.MockListItems)
                .Using(new ListItemEqualityComparer()));
        }

        // It is not possible to send a invalid GUID object, but it can easily happen when using the API
        // The server/client has to be used to test this.
        // The argument validity check is handled by the action filter
        [Test]
        public async Task Get_InvalidGuidFormat_ReturnsBadRequest()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse = await client.GetAsync("http://localhost:57187/api/v1/items/1");

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
                await _itemsRepository.DidNotReceive().GetAsync(Arg.Any<Guid>());
            }
        }

        [Test]
        public async Task Get_NonExistingItem_ReturnsNotFound()
        {
            var nonExistingGuid = Guid.Parse("00000000-0000-0000-0000-000000000004");
            _itemsRepository.GetAsync(nonExistingGuid).ReturnsNull();

            var receivedResponse = await _itemsController.GetItemAsync(nonExistingGuid);

            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Get_ExistingItem_ReturnsCorrectly()
        {
            var targetGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            _itemsRepository.GetAsync(targetGuid).Returns(Utils.Constants.MockListItems.ElementAt(1));

            var receivedResponse = await _itemsController.GetItemAsync(targetGuid);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);
        }

        [Test]
        public async Task Get_ReturnsSpecificExistingItem()
        {
            var targetGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var expectedItem = Utils.Constants.MockListItems.ElementAt(1);
            _itemsRepository.GetAsync(targetGuid).Returns(Utils.Constants.MockListItems.ElementAt(1));

            var receivedResponse = await _itemsController.GetItemAsync(targetGuid);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedResponse).Content;

            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region POST tests

        // The server/client has to be used to test this.
        // The null argument check is handled by the action filter
        [Test]
        public async Task Post_NullText_ReturnsBadRequest()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse =
                    await client.PostAsJsonAsync<string>(new Uri("http://localhost:57187/api/v1/items"), null);

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
                await _itemsRepository.DidNotReceive().AddAsync(Arg.Any<Guid>(), Arg.Any<ListItem>());
            }
        }

        [Test]
        public async Task Post_ValidText_ReturnsPostedItemAndLocation()
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
        public async Task Post_CallsAddWithCorrectGuidAndItem()
        {
            await _itemsController.PostItemAsync(PostedItemText);
            
            await _itemsRepository.Received().AddAsync(TheGuid, Arg.Is<ListItem>(
                (arg) => new ListItemEqualityComparer().Equals(arg, PostedItem)));
        }

        [Test]
        public async Task Post_Returns500OnDuplicitKeyGeneration()
        {
            _itemsRepository.When(x => x.AddAsync(TheGuid, Arg.Any<ListItem>()))
                .Do(x => throw new DuplicateKeyException(TheGuid, "Duplicate GUID generated!"));

            var receivedResponse = await _itemsController.PostItemAsync(PostedItemText);
            Assert.IsInstanceOf<ExceptionResult>(receivedResponse);

            await _itemsRepository.Received().AddAsync(TheGuid, Arg.Is<ListItem>(
                (arg) => new ListItemEqualityComparer().Equals(arg, PostedItem)));
        }

        #endregion

        #region PUT tests

        [Test]
        // The server/client has to be used to test this.
        // The null argument check is handled by the action filter
        public async Task Put_NullArgument_ReturnsBadRequest()
        {
            var config = new HttpConfiguration();
            config = ServerInit.InitializeConfiguration(config);

            using (var server = new HttpServer(config))
            using (var client = new HttpClient(server))
            {
                var receivedResponse = await client.PutAsJsonAsync<ListItem>(
                    new Uri("http://localhost:57187/api/v1/items/00000000-0000-0000-0000-000000000004"), null);

                Assert.AreEqual(receivedResponse.StatusCode, HttpStatusCode.BadRequest);
                await _itemsRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
                await _itemsRepository.DidNotReceive().AddAsync(Arg.Any<Guid>(), Arg.Any<ListItem>());
            }
        }

        // The server/client has to be used to test this.
        // The ID consistency check is handled by the action filter
        [Test]
        public async Task Put_InconsistentIDs_ReturnsBadRequest()
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
                await _itemsRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
                await _itemsRepository.DidNotReceive().AddAsync(Arg.Any<Guid>(), Arg.Any<ListItem>());
            }
        }

        [Test]
        public async Task Put_ValidItem_ReturnsPutItemAndLocation()
        {
            var receivedResponse = await _itemsController.PutItemAsync(TheGuid, PostedItem);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var typedResponse = (CreatedNegotiatedContentResult<ListItem>) receivedResponse;
            var receivedItem = typedResponse.Content;
            var receivedLocation = typedResponse.Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }
        
        [Test]
        public async Task Put_NewValidItem_CallsOnlyAdd()
        {
            _itemsRepository.GetKeysAsync().Returns(new List<Guid>());

            var receivedPutResponse = await _itemsController.PutItemAsync(TheGuid, PostedItem);

            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);
            await _itemsRepository.Received().AddAsync(TheGuid, PostedItem);
            await _itemsRepository.DidNotReceive().DeleteAsync(TheGuid);
        }

        [Test]
        public async Task Put_ValidItemToExistingGuid_CallsAddAndDelete()
        {
            _itemsRepository.GetKeysAsync().Returns(Utils.Constants.MockListItems.Select(item => item.Id));

            var conflictingGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var conflictingListItem = new ListItem
            {
                Id = conflictingGuid,
                Text = "Take a break"
            };

            var receivedPutResponse = await _itemsController.PutItemAsync(conflictingGuid, conflictingListItem);

            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPutResponse);
            await _itemsRepository.Received().AddAsync(conflictingGuid, conflictingListItem);
            await _itemsRepository.Received().DeleteAsync(conflictingGuid);
        }

        [Test]
        public async Task Put_ValidCollection_ReturnsPutCollectionAnLocation()
        {
            var postedCollection = new List<ListItem> {PostedItem};
            _itemsRepository.GetAllAsync().Returns(postedCollection);

            var receivedResponse = await _itemsController.PutItemsCollectionAsync(postedCollection);
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

            await _itemsController.PutItemsCollectionAsync(postedCollection);

            await _itemsRepository.Received(1).ClearAsync();
            await _itemsRepository.Received(1).AddAsync(PostedItem.Id, PostedItem);
            await _itemsRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }

        #endregion

        #region DELETE tests

        [Test]
        public async Task Delete_NonExistingItem_ReturnsNotFound()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000004");
            _itemsRepository
                .When(x => x.DeleteAsync(theGuid))
                .Do(x => throw new KeyNotFoundException());

            var receivedResponse = await _itemsController.DeleteItemAsync(theGuid);

            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
            await _itemsRepository.Received().DeleteAsync(theGuid);
        }

        [Test]
        public async Task Delete_ExistingItem_ReturnsOk()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var receivedResponse = await _itemsController.DeleteItemAsync(theGuid);

            Assert.IsInstanceOf<OkResult>(receivedResponse);
            await _itemsRepository.Received().DeleteAsync(theGuid);
        }

        [Test]
        public async Task Delete_ExistingItem_CallsRepoDelete()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var receivedDeleteResponse = await _itemsController.DeleteItemAsync(theGuid);

            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);
            await _itemsRepository.Received().DeleteAsync(theGuid);
        }

        [Test]
        public async Task Delete_NonExistingIdInCollection_ReturnsNotFound()
        {
            _itemsRepository.GetKeysAsync().Returns(Utils.Constants.MockListItems.Select(item => item.Id));

            var receivedDeleteResponse = await _itemsController.DeleteItemsAsync(new List<Guid>
            {
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Guid.Parse("00000000-0000-0000-0000-000000000005")
            });

            Assert.AreEqual(((NegotiatedContentResult<string>)receivedDeleteResponse).StatusCode, HttpStatusCode.NotFound);
            await _itemsRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }

        [Test]
        public async Task Delete_ValidIdCollection_CallsRepoDelete()
        {
            _itemsRepository.GetKeysAsync().Returns(Utils.Constants.MockListItems.Select(item => item.Id));
            var guid1 = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var guid2 = Guid.Parse("00000000-0000-0000-0000-000000000002");

            var receivedDeleteResponse = await _itemsController.DeleteItemsAsync(new List<Guid>
            {
                guid1,
                guid2               
            });

            Assert.IsInstanceOf<OkResult>(receivedDeleteResponse);
            await _itemsRepository.Received().DeleteAsync(guid1);
            await _itemsRepository.Received().DeleteAsync(guid2);
        }

        #endregion

        #region PATCH tests

        [Test]
        public async Task Patch_NonExistingItem_ReturnsNotFound()
        {
            var theGuid = Guid.Parse("00000000-0000-0000-0000-000000000005");
            _itemsRepository.GetAsync(theGuid).ReturnsNull();

            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", "Buy some aubergine!");

            var receivedResponse = await _itemsController.PatchItemAsync(theGuid, patch);

            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
            await _itemsRepository.DidNotReceive().AddAsync(Arg.Any<Guid>(), Arg.Any<ListItem>());
            await _itemsRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
        }

        [Test]
        public async Task Patch_ValidPatchItem_ReturnsUpdatedListItem()
        {
            _itemsRepository.GetAsync(Guid.Empty).Returns(Utils.Constants.MockListItems.ElementAt(0));

            var newText = "Take a bath";
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = newText
            };

            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", newText);

            var receivedResponse = await _itemsController.PatchItemAsync(Guid.Empty, patch);
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((OkNegotiatedContentResult<ListItem>) receivedResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Patch_CallsAddAndDeleteOnValidGuid()
        {
            _itemsRepository.GetAsync(Guid.Empty).Returns(Utils.Constants.MockListItems.ElementAt(0));

            var newText = "Take a bath";
            var expectedItem = new ListItem
            {
                Id = Guid.Empty,
                Text = newText
            };

            var patch = new JsonPatchDocument<ListItem>();
            patch.Replace("/Text", newText);

            var receivedResponse = await _itemsController.PatchItemAsync(Guid.Empty, patch);

            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);
            await _itemsRepository.Received().DeleteAsync(Guid.Empty);
            await _itemsRepository.Received().AddAsync(Guid.Empty, Arg.Is<ListItem>(
                (arg) => new ListItemEqualityComparer().Equals(arg, expectedItem)));
        }

        [Test]
        public async Task Patch_ForbiddenOperation_ReturnsForbidden()
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
                await _itemsRepository.DidNotReceive().AddAsync(Arg.Any<Guid>(), Arg.Any<ListItem>());
                await _itemsRepository.DidNotReceive().DeleteAsync(Arg.Any<Guid>());
            }
        }
        #endregion
    }
}
