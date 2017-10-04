using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using MongoDB.Driver;

namespace ListApp.Repositories
{
    internal class ListItemRepository : IRepository
    {
        private readonly IMongoDatabase _database;

        public ListItemRepository(string connection)
        {
            var client = new MongoClient(connection);
            _database = client.GetDatabase("listappdb");
        }

        public async Task<IEnumerable<Guid>> GetKeysAsync()
        {
            return await _database.GetCollection<ListItem>("listitems")
                .Find(FilterDefinition<ListItem>.Empty)
                .Project<Guid>(Builders<ListItem>.Projection.Include(item => item.Id)).ToListAsync();
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync()
        {
            return await (await _database.GetCollection<ListItem>("listitems")
                .FindAsync(_ => true))
                .ToListAsync();
        }

        public async Task<ListItem> GetAsync(Guid key)
        {
            return (await _database.GetCollection<ListItem>("listitems").FindAsync(e => e.Id == key)).FirstOrDefault();
        }

        public async Task<ListItem> AddAsync(ListItem entity)
        {
            await _database.GetCollection<ListItem>("listitems").InsertOneAsync(entity);
            return entity;
        }

        public async Task<ListItem> DeleteAsync(Guid key)
        {
            return await _database.GetCollection<ListItem>("listitems")
                .FindOneAndDeleteAsync(e => e.Id == key);
        }

        public async Task<ListItem> UpdateAsync(Guid key, ListItem entity)
        {
            await _database.GetCollection<ListItem>("listitems")
                .FindOneAndReplaceAsync(e => e.Id == key, entity);
            return await GetAsync(key);
        }
    }
}