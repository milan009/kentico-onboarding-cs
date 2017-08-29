using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ListApp.Api.Controllers.V1;
using ListApp.Api.Models;

namespace ListApp.Api.Tests
{
    [TestFixture]
    public class ItemsControllerTests
    {
        private static readonly Func<Guid> GuidCreator = () => Guid.Parse("00000000-0000-0000-0000-000000000003");
        private ItemsController _theController;
        
        [SetUp]
        public void SetUp()
        {
            _theController = new ItemsController(GuidCreator);
        }
        
        [Test]
        public async Task Get_ReturnsAllItems()
        {
            var receivedItems = await _theController.GetItems();
            var expectedItems = new List<ListItem>
            {
                new ListItem(Guid.Parse("00000000-0000-0000-0000-000000000000"), "Stretch correctly"),
                new ListItem(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Make a coffey"),
                new ListItem(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Take over the world")
            };

            CollectionAssert.AreEqual(expectedItems, receivedItems);
        }
    }
}
