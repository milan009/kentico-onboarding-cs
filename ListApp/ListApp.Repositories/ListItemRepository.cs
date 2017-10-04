using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ListApp.Repositories
{
    public class ListItemRepository : IRepository<Guid, ListItem>
    {
        private readonly IMongoDatabase _database;

        public ListItemRepository(string connection)
        {
            var client = new MongoClient(connection);
            _database = client.GetDatabase("listappdb");
        }

        public async Task<IEnumerable<Guid>> GetKeysAsync()
        {
            var l = (await _database.GetCollection<ListItem>("listitems")
                .Find(FilterDefinition<ListItem>.Empty)
                .Project<Guid>(Builders<ListItem>.Projection.Include((item => item.Id))).ToListAsync());
            return null;
            /*return await Task.FromResult(Constants.MockListItems.Select(
                listItem => listItem.Id));*/
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync(Func<ListItem, bool> predicate = null)
        {
            return await 
                (await _database.GetCollection<ListItem>("listitems")
                    .FindAsync(Builders<ListItem>.Filter.Where((item) => predicate(item)))
                )
                .ToListAsync();
        }

        public async Task<ListItem> GetAsync(Guid key)
        {
            return (await _database.GetCollection<ListItem>("listitems").FindAsync(Builders<ListItem>.Filter.Eq(e => e.Id, key))).FirstOrDefault();
            /*  return await Task.FromResult(Constants.MockListItems.ElementAt(0));*/
        }

        public async Task AddAsync(Guid key, ListItem entity)
        {
            await _database.GetCollection<ListItem>("listitems").InsertOneAsync(entity);
            //  await Task.CompletedTask;
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