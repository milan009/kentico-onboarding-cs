using System;
using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services
{
    class DeleteItemService : IDeleteItemService
    {
        private readonly IRepository _repository;

        public DeleteItemService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult> DeleteItemAsync(Guid id)
        {
        
        }
    }
}
