using System;
using System.Collections.Generic;
using System.Data.Linq;

namespace ListApp.Api.Repositories
{
    public interface IRepository<TItemType, TKeyType>
    {
        /// <summary>
        /// A method that returns all keys from repository
        /// </summary>
        /// <returns>A collection of all keys</returns>
        IEnumerable<TKeyType> GetKeys();

        /// <summary>
        /// A method that returns matching items. If called without a predicate,
        /// returns all items in the repository.
        /// </summary>
        /// <param name="predicate">A predicate used to select matching items</param>
        /// <returns>A collection of matching items</returns>
        IEnumerable<TItemType> GetAll(Func<TItemType, bool> predicate = null);

        /// <summary>
        /// A method that returns a specific item identified by given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The item with given key, null if no such item exists.</returns>
        TItemType Get(TKeyType key);

        /// <summary>
        /// Stores given entity in the repository under given key.
        /// </summary>
        /// <param name="key">The key used to identify the entity</param>
        /// <param name="entity">The entity to store in the repository</param>
        /// <exception cref="ArgumentNullException">if a null argument is passed</exception>
        /// <exception cref="DuplicateKeyException">if an item with given key already exists</exception>
        void Add(TKeyType key, TItemType entity);

        /// <summary>
        /// Stores given entity in the repository under given key.
        /// </summary>
        /// <param name="key">The key used to identify the entity</param>
        /// <param name="entity">The entity to store in the repository</param>
        /// <exception cref="ArgumentNullException">if a null argument is passed</exception>
        /// <exception cref="DuplicateKeyException">if an item with given key already exists</exception>
        void Delete(TKeyType key);

        /// <summary>
        /// Removes all entities from the repository
        /// </summary>
        void Clear();
    
    }
}
