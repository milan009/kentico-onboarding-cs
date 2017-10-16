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

        public async Task<OperationResult> UpdateItemAsync(ListItem oldItem, ListItem newItem)
        {
            var itemToReplace = new ListItem
            {
                Id = oldItem.Id,
                Created = oldItem.Created,
                Text = newItem.Text,
                LastModified = _timeService.GetCurrentTime()
            };

            var updatedItem = await _listItemRepository.ReplaceAsync(itemToReplace);

            if (updatedItem == null)
            {
                return OperationResult.Failed;
            }

            return OperationResult.CreateSuccessfulResult(updatedItem);
        }

        public async Task<OperationResult> CheckIfItemExistsAsync(ListItem item)
        {
            var existingItem = await _listItemRepository.GetAsync(item.Id);
            if (existingItem == null)
            {
                return OperationResult.Failed;
            }

            return OperationResult.CreateSuccessfulResult(existingItem);
        }
    }
}