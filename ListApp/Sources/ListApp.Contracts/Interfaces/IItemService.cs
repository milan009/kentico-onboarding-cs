using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IInsertItemService
    {
        Task<ListItem> InsertItemAsync(ListItem item);
    }
}