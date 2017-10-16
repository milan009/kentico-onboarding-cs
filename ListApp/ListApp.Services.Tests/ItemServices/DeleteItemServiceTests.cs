using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Services.ItemServices;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace ListApp.Services.Tests.ItemServices
{
    [TestFixture]
    public class DeleteItemServiceTests
    {
        private IDeleteItemService _deleteItemService;
        private IListItemRepository _listItemRepository;

        [SetUp]
        public void SetUp()
        {
            _listItemRepository = Substitute.For<IListItemRepository>();
            _deleteItemService = new DeleteItemService(_listItemRepository);
        }

        [Test]
        public async Task DeleteItemAsync_NonExistingId_ReturnsCorrectOperationResult()
        {
            var deleteGuid = Guid.NewGuid();
            var expectedResult = OperationResult.Failed;
            _listItemRepository.DeleteAsync(Arg.Any<Guid>()).ReturnsNull();

            var deleteResult = await _deleteItemService.DeleteItemAsync(deleteGuid);

            await _listItemRepository.Received(1).DeleteAsync(deleteGuid);
            Assert.That(deleteResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(deleteResult.Item, Is.EqualTo(expectedResult.Item));
        }

        [Test]
        public async Task DeleteItemAsync_ExistingId_ReturnsCorrectOperationResult()
        {
            var deleteGuid = Guid.NewGuid();
            var expectedItem = new ListItem {Id = deleteGuid, Text = "I was deleted"};
            var expectedResult = OperationResult.CreateSuccessfulResult(expectedItem);
            _listItemRepository.DeleteAsync(Arg.Any<Guid>()).Returns(expectedItem);

            var deleteResult = await _deleteItemService.DeleteItemAsync(deleteGuid);

            await _listItemRepository.Received(1).DeleteAsync(deleteGuid);
            Assert.That(deleteResult.Found, Is.EqualTo(expectedResult.Found));
            Assert.That(deleteResult.Item, Is.EqualTo(expectedResult.Item));
        }
    }
}