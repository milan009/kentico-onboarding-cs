using System;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/items/{id:guid?}", Name = RouteName)]
    public class ItemsController : ApiController
    {
        internal const string RouteName = "itemsRoute";

        private readonly IRepository _repository;
        private readonly IRouteHelper _routeHelper;
        private readonly IItemService _itemService;

        public ItemsController(IRepository repository, IRouteHelper routeHelper, IItemService itemService)
        {
            _repository = repository;
            _routeHelper = routeHelper;
            _itemService = itemService;
        }

        public async Task<IHttpActionResult> GetAsync() 
            => Ok(await _repository.GetAllAsync());

        public async Task<IHttpActionResult> GetAsync([FromUri] Guid id) 
            => Ok(await _repository.GetAsync(id));

        public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem)
        {
            if (newItem.Id != Guid.Empty)
                return BadRequest("Posted item has specified ID!");
            
            var addedItem = await _itemService.InsertItemAsync(newItem);
            var location = _routeHelper.GetItemUrl(addedItem.Id);

            return Created(location, addedItem);
        }
        
        public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody] ListItem newItem) 
            => Ok(await _itemService.UpdateItemAsync(newItem));

        public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id) 
            => Ok(await _repository.DeleteAsync(id));
    } 
}
