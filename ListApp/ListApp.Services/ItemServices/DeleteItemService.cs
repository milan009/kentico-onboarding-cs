using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services.ItemServices
{
    internal class DeleteItemService : IDeleteItemService
    {
        private readonly IListItemRepository _listItemRepository;

        public DeleteItemService(IListItemRepository listItemRepository)
        {
            _listItemRepository = listItemRepository;
        }

        public async Task<ListItemDbOperationResult> DeleteItemAsync(Guid id)
        {
            var deletedItem = await _listItemRepository.DeleteAsync(id);

            return deletedItem == null ? ListItemDbOperationResult.Failed : ListItemDbOperationResult.CreateSuccessfulResult(deletedItem);
        }
    }
}