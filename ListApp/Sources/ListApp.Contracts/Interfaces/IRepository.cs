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
        /// A method that returns a specific item identified by given id.
        /// </summary>
        /// <param name="id">The GUID that identifies required item</param>
        /// <returns>The item with given id, null if no such item exists.</returns>
        Task<ListItem> GetAsync(Guid id);

        /// <summary>
        /// Stores given item in the repository.
        /// </summary>
        /// <param name="item">The item to store in the repository</param>
        Task<ListItem> AddAsync(ListItem item);

        /// <summary>
        /// Deletes item with corresponding id. Does nothing when given id does
        /// not correspond to any item as the goal has been met - no item with given
        /// id remains in the repository.
        /// </summary>
        /// <param name="id">The GUID used to identify the item</param>
        Task<ListItem> DeleteAsync(Guid id);

        /// <summary>
        /// Updates item with the same id of given item with sent data
        /// </summary>
        /// <param name="item">The item containing the updated data</param>
        Task<ListItem> UpdateAsync(ListItem item);
    }
}