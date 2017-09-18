using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Models;
using ListApp.Api.Utils;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v{version:apiVersion}/items")]
    public class ItemsController : ApiController
    {
        [Route]
        public async Task<IHttpActionResult> GetAsync()
            => await Task.FromResult(Ok(Constants.MockListItems));

        [Route("{id}")]
        public async Task<IHttpActionResult> GetAsync([FromUri] Guid id) 
            => await Task.FromResult(Ok(Constants.MockListItems.ElementAt(0)));

        [Route]
        public async Task<IHttpActionResult> PostAsync([FromBody] ListItem newItem) 
            => await Task.FromResult(Created(Url.Request.RequestUri + $"/{Constants.NonExistingItemGuid}", Constants.CreatedListItem));

        [Route("{id}")]
        public async Task<IHttpActionResult> PutAsync([FromUri] Guid id, [FromBody]ListItem newItem) 
            => await Task.FromResult(Created(Url.Request.RequestUri + $"/{Constants.NonExistingItemGuid}", Constants.CreatedListItem));

        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync([FromUri] Guid id) 
            => await Task.FromResult(StatusCode(HttpStatusCode.NoContent));
    }
}
