using System;
using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IDeleteItemService
    {
        Task<ListItemDbOperationResult> DeleteItemAsync(Guid id);
    }
}