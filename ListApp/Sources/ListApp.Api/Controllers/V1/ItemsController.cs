using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Utils;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v{version:apiVersion}/items")]
    [Route("", Name = "itemsBaseRoute")]
    public class ItemsController : ApiController
    {
        private readonly IRepository _repository;

        public ItemsController(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IHttpActionResult> GetAsync() 
            => Ok(await _repository.GetAllAsync());

        [Route("{id}")]
        public async Task<IHttpActionResult> GetAsync([FromUri] Guid id) 
            => Ok(await _repository.GetAsync(id));


        public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem)
        {
            await _repository.AddAsync(newItem.Id, newItem);

            return Created(Url.Route("itemsBaseRoute", null) + $"/{Constants.NonExistingItemGuid}", Constants.CreatedListItem);
        }

        [Route("{id}", Name = "itemsPutRoute")]
        public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody] ListItem newItem)
        {
            await _repository.UpdateAsync(id, newItem);
 
            return Created(Url.Route("itemsPutRoute", new {id = Constants.NonExistingItemGuid}), Constants.CreatedListItem);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id)
        {
            await _repository.DeleteAsync(id);
           
            return StatusCode(HttpStatusCode.NoContent);
        }

    } 
}
