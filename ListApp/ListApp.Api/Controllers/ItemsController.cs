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
                return await Task<IEnumerable<ListItem>>.Factory.StartNew(() => Items);
            }

            [Route("{id}")]
            [HttpGet]
            [Filters.ModelValidationActionFilter()]
            public async Task<IHttpActionResult> GetItem(Guid id)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    var theItem = Items.FirstOrDefault((item) => item.Id == id);
                    if (theItem != null)
                    {
                        return Ok(theItem);
                    }

                    return NotFound();
                });
            }

            [Route]
            [HttpPost]
            public async Task<IHttpActionResult> PostItem([FromBody]string newItemText)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    var createdItem = new ListItem{Id = _idGenerator(), Text = newItemText};

                    Items.Add(createdItem);
                    
                    return Created($"/items/{createdItem.Id}", createdItem);
                });
            }

            [Route]
            [HttpPut]
            public async Task<IHttpActionResult> PutItemsCollection([FromBody] IEnumerable<ListItem> items)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    Items.Clear();
                    var listItems = items as IList<ListItem> ?? items.ToList();
                    Items.AddRange(listItems);

                    return Created("/items", listItems);
                });
            }

            [Route("{id}")]
            [HttpPut]
            [PutGuidConsistencyActionFilter]
            public async Task<IHttpActionResult> PutItem(Guid id, [FromBody] ListItem newItem)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    var existingItemIndex = Items.FindIndex((item) => item.Id == id);
                    if(existingItemIndex == -1)
                    {
                        Items.Add(newItem);
                        return Created($"/items/{id}", newItem);
                    }

                    Items[existingItemIndex] = newItem;
                    return Ok(newItem);
                });
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItem(Guid id)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    var existingItem = Items.FirstOrDefault((item) => item.Id == id);
                    if (existingItem == null)
                    {
                        return NotFound();
                    }

                    Items.Remove(existingItem);

                    return Ok();
                });
            }

            [Route("{id}")]
            [HttpPatch]
            public async Task<IHttpActionResult> PatchItem(Guid id, [FromBody] JsonPatch.JsonPatchDocument<ListItem> patch)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    var existingItem = Items.FirstOrDefault((item) => item.Id == id);
                    if (existingItem == null)
                    {
                        return NotFound();
                    }

                    patch.ApplyUpdatesTo(existingItem);

                    return Ok(existingItem);
                });
            }
        }
    }
}
