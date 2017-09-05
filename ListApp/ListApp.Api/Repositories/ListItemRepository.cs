using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using ListApp.Api.Models;

namespace ListApp.Api.Repositories
{
    public class ListItemRepository : IRepository<ListItem, Guid>
    {
        private static Dictionary<Guid, ListItem> _items;
        public IEnumerable<ListItem> GetAll(Func<ListItem, bool> predicate = null)
        {
            return predicate == null ? _items.Select(pair => pair.Value) : _items.Select(pair => pair.Value).Where(predicate);
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
    }
}