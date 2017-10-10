using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IUpdateItemService
    {
        Task<ListItem> UpdateItemAsync(ListItem item);
    }
}