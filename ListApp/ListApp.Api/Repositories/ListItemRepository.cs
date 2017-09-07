using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;
using ListApp.Api.Models;

namespace ListApp.Api.Repositories
{
    public class ListItemRepository : IRepository<ListItem, Guid>
    {
        // Dummy data holder
        private static readonly Dictionary<Guid, ListItem> _items;

        static ListItemRepository()
        {
            _items = new Dictionary<Guid, ListItem>();

            foreach (var item in Utils.Constants.MockListItems)
            {
                _items.Add(item.Id, item);
            }
        }

        public async Task<IEnumerable<Guid>> GetKeysAsync()
        {
            return await Task.FromResult(_items.Keys.ToList());
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync(Func<ListItem, bool> predicate = null)
        {
            return predicate == null ? await Task.FromResult(_items.Values) 
                : await Task.FromResult(_items.Values.Where(predicate));
        }

        public async Task<ListItem> GetAsync(Guid key)
        {
            return _items.TryGetValue(key, out ListItem item) ? await Task.FromResult(item) 
                : await Task.FromResult<ListItem>(null);
        }

        public async Task AddAsync(Guid key, ListItem entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "ListItem cannot be null!");
            }

            if (_items.ContainsKey(key))
            {
                throw new DuplicateKeyException(key, "Given GUID already exists");
            }

            _items.Add(key, entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid key)
        {
            if (!_items.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Element with \"{key}\" GUID not found!");
            }
            
            _items.Remove(key);
            await Task.CompletedTask;
        }

        public async Task ClearAsync()
        {
            _items.Clear();
            await Task.CompletedTask;
        }
    }
}