using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IUpdateItemService
    {
        Task<OperationResult> UpdateItemAsync(ListItem newItem);
        Task<OperationResult> PrepareUpdatedItem(ListItem newItem);
    }
}