﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using MongoDB.Driver;

namespace ListApp.Repositories
{
    internal class ListItemRepository : IRepository
    {
        private const string DatabaseName = "listappdb";
        private const string CollectionName = "listitems";
        private readonly IMongoDatabase _database;

        public ListItemRepository(DatabaseConfiguration configuration)
        {
            var client = new MongoClient(configuration.ConnectionString);
            _database = client.GetDatabase(DatabaseName);
        }

        public async Task<IEnumerable<ListItem>> GetAllAsync()
        {
            return await (await _database.GetCollection<ListItem>(CollectionName)
                .FindAsync(_ => true))
                .ToListAsync();
        }

        public async Task<ListItem> GetAsync(Guid id)
        {
            return (await _database.GetCollection<ListItem>(CollectionName)
                .FindAsync(dbItem => dbItem.Id == id))
                .FirstOrDefault();
        }

        public async Task<ListItem> AddAsync(ListItem item)
        {
            await _database.GetCollection<ListItem>(CollectionName)
                .InsertOneAsync(item);

            return item;
        }

        public async Task<ListItem> DeleteAsync(Guid id)
        {
            return await _database.GetCollection<ListItem>(CollectionName)
                .FindOneAndDeleteAsync(dbItem => dbItem.Id == id);
        }

        public async Task<ListItem> ReplaceAsync(ListItem item)
        {
            await _database.GetCollection<ListItem>(CollectionName)
                .FindOneAndReplaceAsync(dbItem => dbItem.Id == item.Id, item);

            return item;
        }
    }
}