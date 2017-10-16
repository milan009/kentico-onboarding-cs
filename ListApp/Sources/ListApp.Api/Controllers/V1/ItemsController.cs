using System;
using System.Threading;
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

        public ItemsController(
            IRepository repository, 
            IRouteHelper routeHelper, 
            IInsertItemService insertItemService, 
            IDeleteItemService deleteItemService, 
            IUpdateItemService updateItemService)
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
            ValidateId(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var theItem = await _repository.GetAsync(id);
            if (theItem == null)
                return NotFound();

            return Ok(theItem);
        }
           
        public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem)
        {
            ValidatePostParam(newItem);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var addedItem = await _insertItemService.InsertItemAsync(newItem);
            var location = _routeHelper.GetItemUrl(addedItem.Id);

            return Created(location, addedItem);
        }

        public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody] ListItem newItem)
        {
            ValidatePutParams(id, newItem);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var checkResult = await _updateItemService.CheckIfItemExistsAsync(newItem);
            if (checkResult.Found)
            {
                var updatedResult = await _updateItemService.UpdateItemAsync(checkResult.Item, newItem);
                return Ok(updatedResult.Item);
            }

            var createdItem = await _insertItemService.InsertItemAsync(newItem);

            return Created(_routeHelper.GetItemUrl(createdItem.Id), createdItem);
        }

        public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id)
        {
            ValidateId(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deleteResult = await _deleteItemService.DeleteItemAsync(id);

            if (deleteResult.Found)
                return Ok(deleteResult.Item);

            return NotFound();
        }

        // Validation

        private void ValidatePutParams(Guid id, ListItem item)
        { 
            ValidateId(id);
            ValidateItem(item);

            if(item != null)
                ValidateIdConsistenty(id, item);
        }

        private void ValidatePostParam(ListItem item)
        {
            ValidateItem(item);

            if(ModelState.IsValid && item.Id != Guid.Empty)
                ModelState.AddModelError("IdNotEmpty", "ID has to be empty guid in POST!");
        }

        private void ValidateIdConsistenty(Guid id, ListItem item)
        {
            if(id != item.Id)
                ModelState.AddModelError("IDsNotConsistent", "IDs in URL and item do not match!");
        }

        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
                ModelState.AddModelError("GuidEmpty", "ID cannot be empty guid!");
        }

        private void ValidateItem(ListItem item)
        {
            if (item == null)
            {
                ModelState.AddModelError("ItemNull", "Item cannot be null!");
                return ;
            }

            if (string.IsNullOrWhiteSpace(item.Text))
            {
                ModelState.AddModelError("ItemTextInvalid", "The text of the item is either null, empty, or composed only of white spaces!");
            }
        }
    } 
}
