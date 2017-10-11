using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IUpdateItemService
    {
        Task<OperationResult> UpdateItemAsync(ListItem oldItem, ListItem newItem);
        Task<OperationResult> CheckIfItemExistsAsync(ListItem item);
    }
}