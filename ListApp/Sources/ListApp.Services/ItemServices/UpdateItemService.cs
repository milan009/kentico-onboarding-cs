using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services.ItemServices
{
    internal class UpdateItemService : IUpdateItemService
    {
        private readonly IListItemRepository _listItemRepository;
        private readonly ITimeService _timeService;

        public UpdateItemService(IListItemRepository listItemRepository, ITimeService timeService)
        {
            _listItemRepository = listItemRepository;
            _timeService = timeService;
        }

        public async Task<ListItemDbOperationResult> UpdateItemAsync(ListItem newItem)
        {
            var prepareOperationResult = await PrepareUpdatedItemAsync(newItem);
            if (prepareOperationResult == ListItemDbOperationResult.Failed)
            {
                return prepareOperationResult;
            }

            var updatedItem = await _listItemRepository.ReplaceAsync(prepareOperationResult.Item);

            if (updatedItem == null)
            {
                return ListItemDbOperationResult.Failed;
            }

            return ListItemDbOperationResult.CreateSuccessfulResult(updatedItem);
        }

        internal async Task<ListItemDbOperationResult> PrepareUpdatedItemAsync(ListItem newItem)
        {
            var existingItem = await _listItemRepository.GetAsync(newItem.Id);
            if (existingItem == null)
            {
                return ListItemDbOperationResult.Failed;
            }

            var itemToReplace = new ListItem
            {
                Id = existingItem.Id,
                Created = existingItem.Created,
                Text = newItem.Text,
                LastModified = _timeService.GetCurrentTime()
            };

            return ListItemDbOperationResult.CreateSuccessfulResult(itemToReplace);
        }
    }
}