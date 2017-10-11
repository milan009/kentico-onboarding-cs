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
    public class InsertItemServiceTests
    {
        private IInsertItemService _insertItemService;
        private IRepository _repository;
        private IGuidGenerator _guidGenerator;
        private ITimeHelper _timeHelper;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IRepository>();
            _guidGenerator = Substitute.For<IGuidGenerator>();
            _timeHelper = Substitute.For<ITimeHelper>();

            _insertItemService = new InsertItemService(_repository, _guidGenerator, _timeHelper);
        }

        [Test]
        public async Task InsertItemAsync_NonExistingId_ReturnsCorrectOperationResult()
        {
            var expectedItem = new ListItem
            {
                Id = Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921"),
                Text = "Order pizza",
                Created = DateTime.Parse("17.12.2017"),
                LastModified = DateTime.Parse("17.12.2017")
            };
            var itemToInsert = new ListItem {Id = Guid.Empty, Text = "Order pizza"};
            _guidGenerator.GenerateGuid().Returns(Guid.Parse("9584B1D0-2333-4A0E-A49A-66B45D258921"));
            _timeHelper.GetCurrentTime().Returns(DateTime.Parse("17.12.2017"));
            _repository.AddAsync(Arg.Is<ListItem>(item => item.IsEqualTo(expectedItem)))
                .Returns(expectedItem);

            var insertResult = await _insertItemService.InsertItemAsync(itemToInsert);

            await _repository.Received(1).AddAsync(itemToInsert);
            _timeHelper.Received(1).GetCurrentTime();
            _guidGenerator.Received(1).GenerateGuid();
            Assert.That(insertResult, Is.EqualTo(expectedItem));
        }
    }
}