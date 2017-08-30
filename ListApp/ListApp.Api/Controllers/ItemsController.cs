using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Filters;
using Microsoft.Web.Http;
using ListItem = ListApp.Api.Models.ListItem;

namespace ListApp.Api.Controllers
{
    namespace V1
    {
        [ApiVersion("1.0")]
        [RoutePrefix("api/v{version:apiVersion}/items")]
        public class ItemsController : ApiController
        {
            private static readonly List<ListItem> Items = new List<ListItem>(Utils.Constants.MockListItems);
            private readonly Func<Guid> _idGenerator;

            // Paramless constructor will be using Guid.NewGuid to generate GUIDs
            public ItemsController() : this(Guid.NewGuid) { }

            public ItemsController(Func<Guid> idGenerator)
            {
                _idGenerator = idGenerator;
            }

            // HTTP verbs implementations

            [Route]
            [HttpGet]
            public async Task<IEnumerable<ListItem>> GetItems()
            {
                return await Task.FromResult<IEnumerable<ListItem>>(Items);
            }

            [Route("{id}")]
            [HttpGet]
            [Filters.ModelValidationActionFilter()]
            public async Task<IHttpActionResult> GetItem(Guid id)
            {
                var theItem = Items.FirstOrDefault((item) => item.Id == id);
                if (theItem != null)
                {
                    return Ok(theItem);
                }

                return await Task.FromResult<IHttpActionResult>(NotFound());
            }

            [Route]
            [HttpPost]
            public async Task<IHttpActionResult> PostItem([FromBody]string newItemText)
            {
                var createdItem = new ListItem{Id = _idGenerator(), Text = newItemText};

                Items.Add(createdItem);
                    
                return await Task.FromResult<IHttpActionResult>(Created($"/items/{createdItem.Id}", createdItem));
            }

            [Route]
            [HttpPut]
            public async Task<IHttpActionResult> PutItemsCollection([FromBody] IEnumerable<ListItem> items)
            {
                Items.Clear();
                var listItems = items as IList<ListItem> ?? items.ToList();
                Items.AddRange(listItems);

                return await Task.FromResult<IHttpActionResult>(Created("/items", listItems));
            }

            [Route("{id}")]
            [HttpPut]
            [PutGuidConsistencyActionFilter]
            public async Task<IHttpActionResult> PutItem(Guid id, [FromBody] ListItem newItem)
            {
                var existingItemIndex = Items.FindIndex((item) => item.Id == id);
                if(existingItemIndex == -1)
                {
                    Items.Add(newItem);
                    return await Task.FromResult<IHttpActionResult>(Created($"/items/{id}", newItem));
                }

                Items[existingItemIndex] = newItem;
                return await Task.FromResult<IHttpActionResult>(Ok(newItem));
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItem(Guid id)
            {
                var existingItem = Items.FirstOrDefault((item) => item.Id == id);
                if (existingItem == null)
                {
                    await Task.FromResult<IHttpActionResult>(NotFound());
                }

                Items.Remove(existingItem);

                return await Task.FromResult<IHttpActionResult>(Ok());
            }

            [Route("{id}")]
            [HttpPatch]
            public async Task<IHttpActionResult> PatchItem(Guid id, [FromBody] JsonPatch.JsonPatchDocument<ListItem> patch)
            {
                var existingItem = Items.FirstOrDefault((item) => item.Id == id);
                if (existingItem == null)
                {
                    await Task.FromResult<IHttpActionResult>(NotFound());
                }

                patch.ApplyUpdatesTo(existingItem);

                return await Task.FromResult<IHttpActionResult>(Ok(existingItem));
            }
        }
    }
}
