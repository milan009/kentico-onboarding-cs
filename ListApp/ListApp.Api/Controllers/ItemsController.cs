using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using ListApp.Api.Models;
using ListApp.Api.Utils;

namespace ListApp.Api.Controllers
{
    namespace V1
    {
        [ApiVersion("1.0")]
        [RoutePrefix("api/v{version:apiVersion}/items")]
        public class ItemsController : ApiController
        {
            #region HTTP verbs implementations

            [Route]
            public async Task<IHttpActionResult> GetAsync()
                => await Task.FromResult(Ok(Constants.MockListItems));

            [Route("{id}")]
            public async Task<IHttpActionResult> GetAsync([FromUri]Guid id) 
                => await Task.FromResult(Ok(Constants.MockListItems.ElementAt(0)));

            [Route]
            public async Task<IHttpActionResult> PostAsync([FromBody]ListItem newItem) 
                => await Task.FromResult(Created($"/items/{newItem.Id}", newItem));

            [Route("{id}")]
            public async Task<IHttpActionResult> PutAsync([FromUri]Guid id, [FromBody]ListItem newItem) 
                => await Task.FromResult(Created($"/items/{id}", newItem));

            [Route("{id}")]
            public async Task<IHttpActionResult> DeleteAsync([FromUri]Guid id) 
                => await Task.FromResult(StatusCode(HttpStatusCode.NoContent));

            #endregion
        }
    }
}
