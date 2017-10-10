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

        public async Task<OperationResult> UpdateItemAsync(ListItem item)
        {
            var existingItem = await _repository.GetAsync(item.Id);
            if (existingItem == null)
            {
                var now = _timeHelper.GetCurrentTime();
                item.Created = now;
                item.LastModified = now;

                var addedItem = await _repository.AddAsync(item);
                
                return new OperationResult(false, addedItem);
            }

            existingItem.LastModified = _timeHelper.GetCurrentTime();
            existingItem.Text = item.Text;
            var updatedItem = await _repository.ReplaceAsync(existingItem);

            return new OperationResult(true, updatedItem);
        }
    }
}
