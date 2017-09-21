using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Interfaces;
using ListApp.Api.Models;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v{version:apiVersion}/items")]
    public class ItemsController : ApiController
    {
        #region HTTP verbs implementations

        private readonly IRepository<Guid, ListItem> _repository;
        private readonly IGuidGenerator _guidGenerator;


        public ItemsController(IRepository<Guid, ListItem> repository, IGuidGenerator guidGenerator)
        {
            _repository = repository;
            _guidGenerator = guidGenerator;
        }

        [Route]
        public async Task<IHttpActionResult> GetAsync()
            => Ok(await _repository.GetAllAsync());

        [Route("{id}")]
        public async Task<IHttpActionResult> GetAsync([FromUri] Guid id) 
            => Ok(await _repository.GetAsync(id));

        [Route]
        public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem)
        {
            // Generate the guid - dummy functionality, no need to do checks.
            newItem.Id = _guidGenerator.GenerateGuid();
            await _repository.AddAsync(newItem.Id, newItem);

            return Created(Created(Url.Request.RequestUri + $"/{Constants.NonExistingItemGuid}", Constants.CreatedListItem));
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody] ListItem newItem)
        {
            await _repository.DeleteAsync(id);
            await _repository.AddAsync(id, newItem);

            return Created(Created(Url.Request.RequestUri + $"/{Constants.NonExistingItemGuid}", Constants.CreatedListItem));
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id)
        {
            await _repository.DeleteAsync(id);

            return StatusCode(HttpStatusCode.NoContent);
        }

    } 
}
