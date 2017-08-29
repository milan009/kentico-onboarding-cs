using System;
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

        private ItemsController _theController;

        [SetUp]
        public void SetUp()
        {
            _theController = new ItemsController(GuidCreator);
        }

        #region GET tests
        
        [Test]
        public async Task Get_ReturnsAllDefaultItems()
        {
            var receivedItems = await _theController.GetItems();
            Assert.That(receivedItems, Is.EqualTo(Constants.MockListItems).Using(new ListItemEqualityComparer()));
        }

        [Test]
        public async Task Get_Returns400OnInvalidGuidFormat()
        {
            var receivedResponse = await _theController.GetItem("17");
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(receivedResponse);
        }

        [Test]
        public async Task Get_Returns404OnNonExistingItem()
        {
            var receivedResponse = await _theController.GetItem("00000000-0000-0000-0000-000000000004");
            Assert.IsInstanceOf<NotFoundResult>(receivedResponse);
        }

        [Test]
        public async Task Get_ReturnsSpecificExistingItem()
            {
                var expectedItem = Constants.MockListItems[1];

                var receivedResponse = await _theController.GetItem("00000000-0000-0000-0000-000000000001");
                Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedResponse);

                var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedResponse).Content;
                Assert.That(receivedItem, Is.EqualTo(expectedItem).Using(new ListItemEqualityComparer()));
            }

        #endregion

        #region POST tests

        [Test]
        public async Task Post_Returns400OnNullText()
        {
            var receivedResponse = await _theController.PostItem(null);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(receivedResponse);
        }

        [Test]
        public async Task Post_ReturnsPostedItem()
        {
            var receivedResponse = await _theController.PostItem(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Content;
            var receivedLocation = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }

        [Test]
        public async Task Post_AddsPostedItemCorrectly()
        {
            var receivedPostResponse = await _theController.PostItem(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPostResponse);

            var receivedGetResponse = await _theController.GetItem(TheGuid.ToString());
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }

        #endregion

        #region PUT tests
        /*
        [Test]
        public async Task Post_Returns400OnNullText()
        {
            var receivedResponse = await _theController.PostItem(null);
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(receivedResponse);
        }

        [Test]
        public async Task Post_ReturnsPostedItem()
        {
            var receivedResponse = await _theController.PostItem(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedResponse);

            var receivedItem = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Content;
            var receivedLocation = ((CreatedNegotiatedContentResult<ListItem>)receivedResponse).Location;

            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
            Assert.That(receivedLocation.ToString(), Is.EqualTo($"/items/{TheGuid}"));
        }

        [Test]
        public async Task Post_AddsPostedItemCorrectly()
        {
            var receivedPostResponse = await _theController.PostItem(PostedItemText);
            Assert.IsInstanceOf<CreatedNegotiatedContentResult<ListItem>>(receivedPostResponse);

            var receivedGetResponse = await _theController.GetItem(TheGuid.ToString());
            Assert.IsInstanceOf<OkNegotiatedContentResult<ListItem>>(receivedGetResponse);
            var receivedItem = ((OkNegotiatedContentResult<ListItem>)receivedGetResponse).Content;
            Assert.That(receivedItem, Is.EqualTo(PostedItem).Using(new ListItemEqualityComparer()));
        }
        */
        #endregion

    }
}
