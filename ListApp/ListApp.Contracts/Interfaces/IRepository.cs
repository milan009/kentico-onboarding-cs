using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ListApp.Contracts.Interfaces
{
    internal interface IRepository<TKeyType, TItemType>
    {
        /// <summary>
        /// A method that returns all keys from repository
        /// </summary>
        /// <returns>A collection of all keys</returns>
        Task<IEnumerable<TKeyType>> GetKeysAsync();

        /// <summary>
        /// A method that returns matching items. If called without a predicate,
        /// returns all items in the repository.
        /// </summary>
        /// <param name="predicate">A predicate used to select matching items</param>
        /// <returns>A collection of matching items</returns>
        Task<IEnumerable<TItemType>> GetAllAsync(Func<TItemType, bool> predicate = null);

        /// <summary>
        /// A method that returns a specific item identified by given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The item with given key, null if no such item exists.</returns>
        Task<TItemType> GetAsync(TKeyType key);

        /// <summary>
        /// Stores given entity in the repository under given key.
        /// </summary>
        /// <param name="key">The key used to identify the entity</param>
        /// <param name="entity">The entity to store in the repository</param>
        /// <exception cref="ArgumentNullException">if a null argument is passed</exception>
        /// <exception cref="DuplicateKeyException">if an item with given key already exists</exception>
        Task AddAsync(TKeyType key, TItemType entity);

        /// <summary>
        /// Deletes entity with corresponding key. Does nothing when given key does
        /// not correspond to any item - the goal has been met, no item with given
        /// key remains in the repository.
        /// </summary>
        /// <param name="key">The key used to identify the entity</param>
        /// <exception cref="ArgumentNullException">if a null argument is passed</exception>
        Task DeleteAsync(TKeyType key);

        /// <summary>
        /// Removes all entities from the repository
        /// </summary>
        Task ClearAsync();

        // FRESH CODE
        /// <summary>
        /// Updates entity found under given key with given entity
        /// </summary>
        /// <param name="key">Key indetifyign the entity to update</param>
        /// <param name="entity">The entity containing the updated data</param>
        Task UpdateAsync(TKeyType key, TItemType entity);
    }
}