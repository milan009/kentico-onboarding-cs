using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Utils;

namespace ListApp.Repositories
{
    internal class ListItemRepository : IRepository
    {
        public async Task<IEnumerable<Guid>> GetKeysAsync() 
            => await Task.FromResult(Constants.MockListItems
                .Select(listItem => listItem.Id));

        public async Task<IEnumerable<ListItem>> GetAllAsync() 
            => await Task.FromResult(Constants.MockListItems);

        public async Task<ListItem> GetAsync(Guid id) 
            => await Task.FromResult(Constants.MockListItems.ElementAt(0));

        public async Task<ListItem> AddAsync(ListItem item) 
            => await Task.FromResult(Constants.MockListItems.ElementAt(0));

        public async Task<ListItem> DeleteAsync(Guid id) 
            => await Task.FromResult(Constants.MockListItems.ElementAt(0));

        public async Task<ListItem> UpdateAsync(ListItem entity) 
            => await Task.FromResult(Constants.MockListItems.ElementAt(0));
    }
}