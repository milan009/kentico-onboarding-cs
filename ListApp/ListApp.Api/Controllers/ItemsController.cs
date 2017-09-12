using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Interfaces;
using ListApp.Api.Models;
using ListApp.Api.Repositories;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers
{
    namespace V1
    {
        [ApiVersion("1.0")]
        [RoutePrefix("api/v{version:apiVersion}/items")]
        public class ItemsController : ApiController
        {
            #region HTTP verbs implementations

            private IRepository<Guid, ListItem> _repository;

            public ItemsController() : this(new ListItemRepository()) { }

            public ItemsController(IRepository<Guid, ListItem> repository)
            {
                _repository = repository;
            }


            [Route]
            public async Task<IHttpActionResult> GetAsync()
                => Ok(await _repository.GetAllAsync());

            [Route("{id}")]
            public async Task<IHttpActionResult> GetAsync([FromUri]Guid id) 
                => Ok(await _repository.GetAsync(id));

            [Route]
            public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem)
            {
                await _repository.AddAsync(newItem.Id, newItem);

                return Created($"/items/{newItem.Id}", newItem);
            }

            [Route("{id}")]
            public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody] ListItem newItem)
            {
                await _repository.DeleteAsync(id);
                await _repository.AddAsync(id, newItem);

                return Created($"/items/{id}", newItem);
            }

            [Route("{id}")]
            public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id)
            {
                await _repository.DeleteAsync(id);

                return StatusCode(HttpStatusCode.NoContent);
            }

            #endregion
        }
    }
}
