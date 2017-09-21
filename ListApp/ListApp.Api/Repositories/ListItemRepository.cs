using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ListApp.Api.Interfaces;
using ListApp.Api.Models;
using ListApp.Api.Utils;
using MongoDB.Bson;
using MongoDB.Driver;


namespace ListApp.Api.Repositories
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
            return await Task.FromResult(Constants.MockListItems.Select(
                listItem => listItem.Id));
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync(Func<ListItem, bool> predicate = null)
        {
            var a = await _database.GetCollection<BsonDocument>("listitems")
                .Find(new BsonDocument()).ToListAsync();
            var items = a.Select(e => new ListItem {Id = Guid.Parse(e["Id"].AsString), Text = e["Text"].AsString});
                
            //     return await _database.GetCollection<ListItem>("listitems").Find(new BsonDocument()).ToListAsync();

            /*    return predicate == null ? await Task.FromResult(Constants.MockListItems)
                    : await Task.FromResult(Constants.MockListItems.Where(predicate));*/
            return items;
        }

        public async Task<ListItem> GetAsync(Guid key)
        {
            return (await _database.GetCollection<ListItem>("listitems").FindAsync(Builders<ListItem>.Filter.Eq(e => e.Id, key))).FirstOrDefault();
            /*  return await Task.FromResult(Constants.MockListItems.ElementAt(0));*/
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