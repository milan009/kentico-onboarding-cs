using System;
using System.Threading.Tasks;
using NUnit.Framework;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Services.ItemServices;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace ListApp.Services.Tests
{
    [TestFixture]
    public class DeleteItemServiceTests
    {
        private IDeleteItemService _deleteItemService;
        private IRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = Substitute.For<IRepository>();
            _deleteItemService = new DeleteItemService(_repository);
        }

        [Test]
        public async Task DeleteItemAsync_NonExistingId_ReturnsCorrectOperationResult()
        {
            var deleteGuid = Guid.NewGuid();
            var expectedResult = OperationResult.Failed;
            _repository.DeleteAsync(Arg.Any<Guid>()).ReturnsNull();

            var deleteResult = await _deleteItemService.DeleteItemAsync(deleteGuid);

            await _repository.Received(1).DeleteAsync(deleteGuid);
            Assert.That(deleteResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(deleteResult.Item, Is.EqualTo(expectedResult.Item));
        }

        [Test]
        public async Task DeleteItemAsync_ExistingId_ReturnsCorrectOperationResult()
        {
            var deleteGuid = Guid.NewGuid();
            var expectedItem = new ListItem {Id = deleteGuid, Text = "I was deleted"};
            var expectedResult = OperationResult.CreateSuccessfulResult(expectedItem);
            _repository.DeleteAsync(Arg.Any<Guid>()).Returns(expectedItem);

            var deleteResult = await _deleteItemService.DeleteItemAsync(deleteGuid);

            await _repository.Received(1).DeleteAsync(deleteGuid);
            Assert.That(deleteResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(deleteResult.Item, Is.EqualTo(expectedResult.Item));
        }
    }
}