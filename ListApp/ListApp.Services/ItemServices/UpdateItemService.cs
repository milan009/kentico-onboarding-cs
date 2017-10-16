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

        public async Task<OperationResult> UpdateItemAsync(ListItem newItem)
        {
            var updatedItem = await _listItemRepository.ReplaceAsync(newItem);

            if (updatedItem == null)
            {
                return OperationResult.Failed;
            }

            return OperationResult.CreateSuccessfulResult(updatedItem);
        }

        public async Task<OperationResult> PrepareUpdatedItem(ListItem newItem)
        {
            var existingItem = await _listItemRepository.GetAsync(newItem.Id);
            if (existingItem == null)
            {
                return OperationResult.Failed;
            }

            var itemToReplace = new ListItem
            {
                Id = existingItem.Id,
                Created = existingItem.Created,
                Text = newItem.Text,
                LastModified = _timeService.GetCurrentTime()
            };

            return OperationResult.CreateSuccessfulResult(existingItem);
        }
    }
}