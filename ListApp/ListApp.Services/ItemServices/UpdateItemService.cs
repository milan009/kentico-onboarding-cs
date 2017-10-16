using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services.ItemServices
{
    internal class UpdateItemService : IUpdateItemService
    {
        private readonly IRepository _repository;
        private readonly ITimeService _timeService;

        public UpdateItemService(IRepository repository, ITimeService timeService)
        {
            _repository = repository;
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

            var updatedItem = await _repository.ReplaceAsync(itemToReplace);

            if (updatedItem == null)
            {
                return OperationResult.Failed;
            }

            return OperationResult.CreateSuccessfulResult(updatedItem);
        }

        public async Task<OperationResult> CheckIfItemExistsAsync(ListItem item)
        {
            var existingItem = await _repository.GetAsync(item.Id);
            if (existingItem == null)
            {
                return OperationResult.Failed;
            }

            return OperationResult.CreateSuccessfulResult(existingItem);
        }
    }
}