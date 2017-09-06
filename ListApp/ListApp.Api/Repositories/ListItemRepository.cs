using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
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

        public IEnumerable<Guid> GetKeys()
        {
            return _items.Keys;
        }

        public IEnumerable<ListItem> GetAll(Func<ListItem, bool> predicate = null)
        {
            return predicate == null ? _items.Values : _items.Values.Where(predicate);
        }

        public ListItem Get(Guid key)
        {
            return _items.TryGetValue(key, out ListItem item) ? item : null;
        }

        public void Add(Guid key, ListItem entity)
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
        }

        public void Delete(Guid key)
        {
            if (!_items.ContainsKey(key))
            {
                throw new KeyNotFoundException($"Element with \"{key}\" GUID not found!");
            }

            _items.Remove(key);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}