using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Api.Services
{
    internal class InsertItemService : IInsertItemService
    {
        private readonly IRepository _repository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ITimeHelper _timeHelper;

        public InsertItemService(IRepository repository, IGuidGenerator guidGenerator, ITimeHelper timeHelper)
        {
            _repository = repository;
            _guidGenerator = guidGenerator;
            _timeHelper = timeHelper;
        }

        public async Task<ListItem> InsertItemAsync(ListItem item)
        {
            item.Created = _timeHelper.GetCurrentTime();
            item.LastModified = _timeHelper.GetCurrentTime();
            item.Id = _guidGenerator.GenerateGuid();

            return await _repository.AddAsync(item);
        }
    }
}
