using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Web.Http;
using ListApp.Api.Models;

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
                => await Task.FromResult(Ok(new List<ListItem>
                {
                    new ListItem {Id = Guid.Parse("226CFFBC-2B4B-4178-828E-62709FCAB951"), Text = "Stretch correctly"},
                    new ListItem {Id = Guid.Parse("31100C72-C22A-4C8E-98E9-DDAEA5785660"), Text = "Make a coffey"},
                    new ListItem {Id = Guid.Parse("01D90A78-4A61-4E69-A714-A374044C163A"), Text = "Take over the world"}
                }));

            [Route("{id}")]
            public async Task<IHttpActionResult> GetAsync([FromUri]Guid id) 
                => await Task.FromResult(Ok(new ListItem
                {
                      Id = Guid.Parse("226CFFBC-2B4B-4178-828E-62709FCAB951"), Text = "Stretch correctly"
                }));

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
