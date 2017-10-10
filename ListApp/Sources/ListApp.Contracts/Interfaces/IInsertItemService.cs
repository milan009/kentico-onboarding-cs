using System.Threading.Tasks;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IInsertItemService
    {
        Task<OperationResult> InsertItemAsync(ListItem item);
    }
}