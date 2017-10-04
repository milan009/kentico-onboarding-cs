using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IRepository
    {
        /// <summary>
        /// A method that returns all keys from repository
        /// </summary>
        /// <returns>A collection of all keys</returns>
        Task<IEnumerable<Guid>> GetKeysAsync();

        /// <summary>
        /// A method that returns all items in the repository.
        /// </summary>
        /// <returns>A collection of all items</returns>
        Task<IEnumerable<ListItem>> GetAllAsync();

        /// <summary>
        /// A method that returns a specific item identified by given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The item with given key, null if no such item exists.</returns>
        Task<ListItem> GetAsync(Guid key);

        /// <summary>
        /// Stores given entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to store in the repository</param>
        /// <exception cref="ArgumentNullException">if a null argument is passed</exception>
        /// <exception cref="DuplicateKeyException">if an item with given key already exists</exception>
        Task<ListItem> AddAsync(ListItem entity);

        /// <summary>
        /// Deletes entity with corresponding key. Does nothing when given key does
        /// not correspond to any item as the goal has been met - no item with given
        /// key remains in the repository.
        /// </summary>
        /// <param name="key">The key used to identify the entity</param>
        Task<ListItem> DeleteAsync(Guid key);

        /// <summary>
        /// Updates entity found under given key with given entity
        /// </summary>
        /// <param name="key">Key indetifyign the entity to update</param>
        /// <param name="entity">The entity containing the updated data</param>
        Task<ListItem> UpdateAsync(Guid key, ListItem entity);
    }
}