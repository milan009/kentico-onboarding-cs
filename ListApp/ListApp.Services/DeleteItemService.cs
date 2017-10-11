using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services
{
    internal class DeleteItemService : IDeleteItemService
    {
        private readonly IRepository _repository;

        public DeleteItemService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> DeleteItemAsync(Guid id)
        {
            var deletedItem = await _repository.DeleteAsync(id);

            return deletedItem == null ? OperationResult.Failed : OperationResult.CreateSuccessfulResult(deletedItem);
        }
    }
}
