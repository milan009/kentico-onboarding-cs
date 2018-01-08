using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IUpdateItemService
    {
        Task<ListItemDbOperationResult> UpdateItemAsync(ListItem newItem);
    }
}