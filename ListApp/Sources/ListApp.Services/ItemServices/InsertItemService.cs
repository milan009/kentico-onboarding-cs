using System.Threading.Tasks;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Services.ItemServices
{
    internal class InsertItemService : IInsertItemService
    {
        private readonly IListItemRepository _listItemRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ITimeService _timeService;

        public InsertItemService(IListItemRepository listItemRepository, IGuidGenerator guidGenerator, ITimeService timeService)
        {
            _listItemRepository = listItemRepository;
            _guidGenerator = guidGenerator;
            _timeService = timeService;
        }

        public async Task<ListItem> InsertItemAsync(ListItem item)
        {
            var now = _timeService.GetCurrentTime();
            var newItem = new ListItem
            {
                Id = _guidGenerator.GenerateGuid(),
                Text = item.Text,
                Created = now,
                LastModified = now
            };

            return await _listItemRepository.AddAsync(newItem);
        }
    }
}