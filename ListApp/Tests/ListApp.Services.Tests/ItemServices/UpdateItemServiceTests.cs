using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Services.ItemServices;
using ListApp.Tests.Base;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace ListApp.Services.Tests.ItemServices
{
    [TestFixture]
    public class UpdateItemServiceTests
    {
        private IUpdateItemService _updateItemService;
        private IListItemRepository _listItemRepository;
        private ITimeService _timeService;

        [SetUp]
        public void SetUp()
        {
            _listItemRepository = Substitute.For<IListItemRepository>();
            _timeService = Substitute.For<ITimeService>();

            _updateItemService = new UpdateItemService(_listItemRepository, _timeService);
        }

        [Test]
        public async Task UpdateItemAsync_RepoGetReturnsNull_ReturnsCorrectOperationResult()
        {
            //  Arrange
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var itemToUpdate = new ListItem
            {
                Id = guid,
                Text = "Order fries"
            };
            var expectedResult = ListItemDbOperationResult.Failed;

            _listItemRepository.GetAsync(Arg.Any<Guid>())
                .ReturnsNull();

            //  Act
            var updateResult = await _updateItemService.UpdateItemAsync(itemToUpdate);

            //  Assert
            await _listItemRepository.Received(1).GetAsync(Arg.Any<Guid>());
            await _listItemRepository.Received(0).ReplaceAsync(Arg.Any<ListItem>());

            Assert.That(updateResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task UpdateItemAsync_RepoReplaceReturnsNull_ReturnsCorrectOperationResult()
        {
            //  Arrange
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var itemToUpdate = new ListItem
            {
                Id = guid,
                Text = "Order fries"
            };
            var oldItem = new ListItem
            {
                Id = guid,
                Text = "Order pizza",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("7.12.2017")
            };
            var expectedResult = ListItemDbOperationResult.Failed;

            _listItemRepository.GetAsync(Arg.Any<Guid>())
                .Returns(oldItem);
            _listItemRepository.ReplaceAsync(Arg.Any<ListItem>())
                .ReturnsNull();

            //  Act
            var updateResult = await _updateItemService.UpdateItemAsync(itemToUpdate);

            //  Assert
            await _listItemRepository.Received(1).GetAsync(Arg.Any<Guid>());
            await _listItemRepository.Received(1).ReplaceAsync(Arg.Any<ListItem>());

            Assert.That(updateResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task UpdateItemAsync_ExistingItem_ReturnsCorrectOperationResult()
        {
            //  Arrange
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var oldItem = new ListItem
            {
                Id = guid,
                Text = "Order pizza",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("7.12.2017")
            };
            var expectedItem = new ListItem
            {
                Id = guid,
                Text = "Order fries",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var updateItem = new ListItem
            {
                Id = guid,
                Text = "Order fries"
            };
            var expectedResult = ListItemDbOperationResult.CreateSuccessfulResult(expectedItem);

            _listItemRepository.GetAsync(guid)
                .Returns(oldItem);
            _timeService.GetCurrentTime()
                .Returns(DateTime.Parse("17.12.2017"));

            _listItemRepository.ReplaceAsync(Arg.Any<ListItem>())
                .Returns(call => call.Arg<ListItem>());

            //  Act
            var updateResult = await _updateItemService.UpdateItemAsync(expectedItem);

            //  Assert
            await _listItemRepository.Received(1).ReplaceAsync(
                Arg.Is<ListItem>(
                    item => ListItemEqualityComparer.Instance.Equals(item, expectedItem)));

            Assert.That(updateResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(updateResult.Item, Is.EqualTo(expectedResult.Item).UsingListItemComparer());
        }
    }
}