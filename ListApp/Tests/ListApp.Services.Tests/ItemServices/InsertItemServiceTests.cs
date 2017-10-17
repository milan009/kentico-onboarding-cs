﻿using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Services.ItemServices;
using ListApp.Tests.Base;
using NSubstitute;
using NUnit.Framework;

namespace ListApp.Services.Tests.ItemServices
{
    [TestFixture]
    public class InsertItemServiceTests
    {
        private IInsertItemService _insertItemService;
        private IListItemRepository _listItemRepository;
        private IGuidGenerator _guidGenerator;
        private ITimeService _timeService;

        [SetUp]
        public void SetUp()
        {
            _listItemRepository = Substitute.For<IListItemRepository>();
            _guidGenerator = Substitute.For<IGuidGenerator>();
            _timeService = Substitute.For<ITimeService>();

            _insertItemService = new InsertItemService(_listItemRepository, _guidGenerator, _timeService);
        }

        [Test]
        public async Task InsertItemAsync_NonExistingId_ReturnsCorrectOperationResult()
        {
            //  Arrange
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921"),
                Text = "Order pizza",
                Created = DateTime.Parse("17.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var itemToInsert = new ListItem {Id = Guid.Empty, Text = "Order pizza"};

            _guidGenerator.GenerateGuid()
                .Returns(Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921"));
            _timeService.GetCurrentTime()
                .Returns(DateTime.Parse("17.12.2017"));
            _listItemRepository.AddAsync(Arg.Any<ListItem>())
                .Returns(call => call.Arg<ListItem>());

            //  Act
            var insertResult = await _insertItemService.InsertItemAsync(itemToInsert);

            //  Assert
            await _listItemRepository.Received(1).AddAsync(Arg.Is<ListItem>(
                item => ListItemEqualityComparer.Instance.Equals(item, expectedItem)));
            _timeService.Received(1).GetCurrentTime();
            _guidGenerator.Received(1).GenerateGuid();
            Assert.That(insertResult, Is.EqualTo(expectedItem).UsingListItemComparer());
        }
    }
}