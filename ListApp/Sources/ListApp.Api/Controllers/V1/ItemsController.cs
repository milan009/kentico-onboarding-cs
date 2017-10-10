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
        private readonly IInsertItemService _insertItemService;
        private readonly IDeleteItemService _deleteItemService;
        private readonly IUpdateItemService _updateItemService;

        public ItemsController(IRepository repository, IRouteHelper routeHelper, IInsertItemService insertItemService, IDeleteItemService deleteItemService, IUpdateItemService updateItemService)
        {
            _repository = repository;
            _routeHelper = routeHelper;
            _insertItemService = insertItemService;
            _deleteItemService = deleteItemService;
            _updateItemService = updateItemService;
        }

        public async Task<IHttpActionResult> GetAsync() 
            => Ok(await _repository.GetAllAsync());

        public async Task<IHttpActionResult> GetAsync([FromUri] Guid id)
        {
            var theItem = await _repository.GetAsync(id);
            if (theItem == null)
                return NotFound();

            return Ok(theItem);
        }
           

        public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem)
        {
            if (newItem == null)
                return BadRequest("Posted item cannot be null!");

            if (newItem.Id != Guid.Empty)
                return BadRequest("Posted item must have empty guid!");

            var addedItem = await _insertItemService.InsertItemAsync(newItem);
            var location = _routeHelper.GetItemUrl(addedItem.Id);

            return Created(location, addedItem);
        }

        public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody] ListItem newItem)
        {
            if (newItem == null)
                return BadRequest("Posted item cannot be null!");

            if (newItem.Id != id)
                return BadRequest("IDs do not match in URL and posted item!");

            if (newItem.Id == Guid.Empty)
                return BadRequest("ID cannot be empty guid!");

            var updatedResult = await _updateItemService.UpdateItemAsync(newItem);

            if (updatedResult.Found)
                return Ok(updatedResult.Item);

            return Created(_routeHelper.GetItemUrl(updatedResult.Item.Id), updatedResult.Item);
        }

        public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id)
        {
            var deleteResult = await _deleteItemService.DeleteItemAsync(id);

            if (deleteResult.Found)
            {
                return Ok(deleteResult.Item);
            }

            return NotFound();
        }
    } 
}
