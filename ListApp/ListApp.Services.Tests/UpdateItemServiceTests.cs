using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Tests.Base;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework.Constraints;

namespace ListApp.Services.Tests
{
    [TestFixture]
    public class UpdateItemServiceTests
    {
        private IUpdateItemService _updateItemService;
        private IRepository _repository;
        private ITimeHelper _timeHelper;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IRepository>();
            _timeHelper = Substitute.For<ITimeHelper>();

            _updateItemService = new UpdateItemService(_repository, _timeHelper);
        }

        [Test]
        public async Task CheckIfItemExistsAsync_NonExistingId_ReturnsCorrectOperationResult()
        {
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var itemToUpdate = new ListItem
            {
                Id = guid,
                Text = "Order pizza"
            };
            var expectedResult = OperationResult.Failed;

            _repository.GetAsync(Arg.Any<Guid>())
                .ReturnsNull();

            var updateResult = await _updateItemService.CheckIfItemExistsAsync(itemToUpdate);

            await _repository.Received(0).AddAsync(Arg.Any<ListItem>());
            await _repository.Received(1).GetAsync(guid);
            Assert.That(updateResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task CheckIfItemExistsAsync_ExistingId_ReturnsCorrectOperationResult()
        {
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var expectedItem = new ListItem
            {
                Id = guid,
                Text = "Order pizza",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var itemToCheck = new ListItem
            {
                Id = guid,
                Text = "Order fries"
            };

            var expectedResult = OperationResult.CreateSuccessfulResult(expectedItem);
            _repository.GetAsync(guid)
                .Returns(expectedItem);

            var updateResult = await _updateItemService.CheckIfItemExistsAsync(itemToCheck);

            await _repository.Received(1).GetAsync(guid);
            Assert.That(updateResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(updateResult.Item, Is.EqualTo(expectedResult.Item).UsingListItemComparer());
        }

        [Test]
        public async Task UpdateItemAsync_UpdateReturnsNull_ReturnsCorrectOperationResult()
        {
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var oldItem = new ListItem
            {
                Id = guid,
                Text = "Order pizza",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("7.12.2017")
            };
            var itemToUpdate = new ListItem
            {
                Id = guid,
                Text = "Order fries"
            };
            var updatedItem = new ListItem
            {
                Id = guid,
                Text = "Order fries",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var expectedResult = OperationResult.Failed;

            _repository.ReplaceAsync(Arg.Any<ListItem>())
                .ReturnsNull();
            _timeHelper.GetCurrentTime()
                .Returns(DateTime.Parse("17.12.2017"));

            var updateResult = await _updateItemService.UpdateItemAsync(oldItem, itemToUpdate);

            await _repository.Received(1).ReplaceAsync(Arg.Any<ListItem>());
            _timeHelper.Received(1).GetCurrentTime();
            Assert.That(updateResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task UpdateItemAsync_CorrectItem_ReturnsCorrectOperationResult()
        {
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var expectedItem = new ListItem
            {
                Id = guid,
                Text = "Order pizza",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var repoItem = new ListItem
            {
                Id = guid,
                Text = "Order fries",
                Created = DateTime.Parse("5.12.2017"),
                LastModified = DateTime.Parse("7.12.2017")
            };
            var itemToUpdate = new ListItem
            {
                Id = guid,
                Text = "Order pizza"
            };
            var expectedResult = OperationResult.CreateSuccessfulResult(expectedItem);

            _timeHelper.GetCurrentTime()
                .Returns(DateTime.Parse("17.12.2017"));
            _repository.GetAsync(guid)
                .Returns(repoItem);
            _repository.ReplaceAsync(Arg.Any<ListItem>())
                .Returns(call => call.Arg<ListItem>());//expectedItem);

            var updateResult = await _updateItemService.UpdateItemAsync(repoItem, itemToUpdate);

            await _repository.Received(1).ReplaceAsync(
                Arg.Is<ListItem>(
                    item => ListItemEqualityComparer.Instance.Equals(item, expectedItem)));
        
            _timeHelper.Received(1).GetCurrentTime();
            Assert.That(updateResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(updateResult.Item, Is.EqualTo(expectedResult.Item).UsingListItemComparer());
        }
    }
}