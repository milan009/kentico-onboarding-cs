﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Services.Tests.Extensions;
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
        public async Task UpdateItemAsync_NonExistingId_ReturnsCorrectOperationResult()
        {
            var guid = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921");
            var expectedItem = new ListItem
            {
                Id = guid,
                Text = "Order pizza",
                Created = DateTime.Parse("17.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var itemToUpdate = new ListItem
            {
                Id = guid,
                Text = "Order pizza"
            };
            var expectedResult = new OperationResult(false, expectedItem);
            _timeHelper.GetCurrentTime().Returns(DateTime.Parse("17.12.2017"));
            _repository.GetAsync(Arg.Any<Guid>()).ReturnsNull();
            _repository.AddAsync(Arg.Is<ListItem>(item => item.IsEqualTo(expectedItem)))
                .Returns(expectedItem);

            var updateResult = await _updateItemService.UpdateItemAsync(itemToUpdate);

            await _repository.Received(1).AddAsync(itemToUpdate);
            await _repository.Received(1).GetAsync(guid);
            _timeHelper.Received(1).GetCurrentTime();
            Assert.That(updateResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(updateResult.Item, Is.EqualTo(expectedResult.Item));
        }

        [Test]
        public async Task UpdateItemAsync_ExistingId_ReturnsCorrectOperationResult()
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
            var expectedResult = new OperationResult(true, expectedItem);
            _timeHelper.GetCurrentTime().Returns(DateTime.Parse("17.12.2017"));
            _repository.GetAsync(guid).Returns(repoItem);
            _repository.ReplaceAsync(Arg.Is<ListItem>(item => item.IsEqualTo(expectedItem)))
                .Returns(expectedItem);

            var updateResult = await _updateItemService.UpdateItemAsync(itemToUpdate);

            await _repository.Received(1).ReplaceAsync(repoItem);
            await _repository.Received(1).GetAsync(guid);
            _timeHelper.Received(1).GetCurrentTime();
            Assert.That(updateResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(updateResult.Item, Is.EqualTo(expectedResult.Item));
        }
    }
}
