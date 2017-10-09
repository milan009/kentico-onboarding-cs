using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IItemService
    {
        Task<ListItem> InsertItemAsync(ListItem item);
        Task<ListItem> UpdateItemAsync(ListItem item);
    }
}