using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Utils;

namespace ListApp.Repositories
{
    public class ListItemRepository : IRepository<Guid, ListItem>
    {
        public async Task<IEnumerable<Guid>> GetKeysAsync()
        {
            return await Task.FromResult(Constants.MockListItems.Select(
                listItem => listItem.Id));
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync(Func<ListItem, bool> predicate = null)
        {
            return predicate == null ? await Task.FromResult(Constants.MockListItems)
                : await Task.FromResult(Constants.MockListItems.Where(predicate));
        }

        public async Task<ListItem> GetAsync(Guid key)
        {
            return await Task.FromResult(Constants.MockListItems.ElementAt(0));
        }

        public async Task AddAsync(Guid key, ListItem entity)
        {
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid key)
        {
            await Task.CompletedTask;
        }

        public async Task ClearAsync()
        {
            await Task.CompletedTask;
        }
    }
}