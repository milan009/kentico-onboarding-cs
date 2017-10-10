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
        private const string databaseName = "listappdb";
        private const string collectionName = "listitems";
        private readonly IMongoDatabase _database;

        public ListItemRepository(DatabaseConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            _database = client.GetDatabase(databaseName);
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync()
        {
            return await (await _database.GetCollection<ListItem>(collectionName)
                .FindAsync(_ => true))
                .ToListAsync();
        }

        public async Task<ListItem> GetAsync(Guid id)
        {
            return (await _database.GetCollection<ListItem>(collectionName)
                .FindAsync(e => e.Id == id))
                .FirstOrDefault();
        }

        public async Task<ListItem> AddAsync(ListItem item)
        {
            await _database.GetCollection<ListItem>(collectionName)
                .InsertOneAsync(item);

            return item;
        }

        public async Task<ListItem> DeleteAsync(Guid id)
        {
            return await _database.GetCollection<ListItem>(collectionName)
                .FindOneAndDeleteAsync(e => e.Id == id);
        }

        public async Task<ListItem> ReplaceAsync(ListItem item)
        {
            await _database.GetCollection<ListItem>(collectionName)
                .FindOneAndReplaceAsync(e => e.Id == item.Id, item);

            return item;
        }
    }
}