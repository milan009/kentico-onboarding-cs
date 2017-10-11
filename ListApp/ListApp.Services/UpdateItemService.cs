using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services
{
    internal class UpdateItemService : IUpdateItemService
    {
        private readonly IRepository _repository;
        private readonly ITimeHelper _timeHelper;

        public UpdateItemService(IRepository repository, ITimeHelper timeHelper)
        {
            _repository = repository;
            _timeHelper = timeHelper;
        }

        public async Task<OperationResult> UpdateItemAsync(ListItem oldItem, ListItem newItem)
        {
            var itemToReplace = new ListItem
            {
                Id = oldItem.Id,
                Created = oldItem.Created,
                Text = newItem.Text,
                LastModified = _timeHelper.GetCurrentTime()
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
