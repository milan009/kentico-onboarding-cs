using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Repositories
{
    internal class ListItemRepository : IRepository
    {
        private readonly List<ListItem> _mockListItems = new List<ListItem>
        {
            new ListItem {Id = Guid.Parse("BFBF8A62-FD82-42D4-A86B-324704BE161E"), Text = "Stretch correctly"},
            new ListItem {Id = Guid.Parse("A55578BC-57F2-4A42-BEDF-7D8C23992DBC"), Text = "Make coffee"},
            new ListItem {Id = Guid.Parse("C6F4D46F-D7B1-45DD-8C7C-265313AF77BB"), Text = "Take over the world"}
        };

        public async Task<IEnumerable<ListItem>> GetAllAsync() 
            => await Task.FromResult(_mockListItems);

        public async Task<ListItem> GetAsync(Guid id) 
            => await Task.FromResult(_mockListItems.ElementAt(0));

        public async Task<ListItem> AddAsync(ListItem item) 
            => await Task.FromResult(_mockListItems.ElementAt(0));

        public async Task<ListItem> DeleteAsync(Guid id) 
            => await Task.FromResult(_mockListItems.ElementAt(0));

        public async Task<ListItem> UpdateAsync(ListItem entity) 
            => await Task.FromResult(_mockListItems.ElementAt(0));
    }
}