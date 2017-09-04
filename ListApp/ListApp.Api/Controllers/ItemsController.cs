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
            // Currently used to simulate a way to store ListItems
            private static List<ListItem> _items;
            private readonly Func<Guid> _idGenerator;

            // Paramless constructor will be using Guid.NewGuid to generate GUIDs
            public ItemsController() : this(Guid.NewGuid) { }

            public ItemsController(Func<Guid> idGenerator)
            {
                _idGenerator = idGenerator;
            }

            static ItemsController()
            {
                InitializeItems();
            }

            // Initializes the static list of ListItems to default mock value
            public static void InitializeItems()
            {
                _items = new List<ListItem>(Utils.Constants.MockListItems);
            }

            #region HTTP verbs implementations

            [Route]
            [HttpGet]
            public async Task<IEnumerable<ListItem>> GetItems()
            {
                return await Task.FromResult<IEnumerable<ListItem>>(_items);
            }

            [Route("{id}")]
            [HttpGet]
            public async Task<IHttpActionResult> GetItem(Guid id)
            {
                var theItem = _items.FirstOrDefault((item) => item.Id == id);
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

                _items.Add(createdItem);
                    
                return await Task.FromResult<IHttpActionResult>(Created($"/items/{createdItem.Id}", createdItem));
            }

            [Route]
            [HttpPut]
            [PutCollectionActionFilter]
            public async Task<IHttpActionResult> PutItemsCollection([FromBody] IEnumerable<ListItem> items)
            {
                var listItems = items as IList<ListItem> ?? items.ToList();

                if (_items.Any())
                {
                    _items.Clear();
                    _items.AddRange(listItems);
                    return await Task.FromResult<IHttpActionResult>(Ok(_items));
                }

                _items.AddRange(listItems);
                return await Task.FromResult<IHttpActionResult>(Created("/items", _items));
            }

            [Route("{id}")]
            [HttpPut]
            [PutGuidConsistencyActionFilter]
            public async Task<IHttpActionResult> PutItem(Guid id, [FromBody] ListItem newItem)
            {
                var existingItemIndex = _items.FindIndex((item) => item.Id == id);
                if(existingItemIndex == -1)
                {
                    _items.Add(newItem);
                    return await Task.FromResult<IHttpActionResult>(Created($"/items/{id}", newItem));
                }

                _items[existingItemIndex] = newItem;
                return await Task.FromResult<IHttpActionResult>(Ok(newItem));
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItem(Guid id)
            {
                var existingItem = _items.FirstOrDefault((item) => item.Id == id);
                if (existingItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }

                _items.Remove(existingItem);

                return await Task.FromResult<IHttpActionResult>(Ok());
            }

            [Route("{id}")]
            [HttpPatch]
            public async Task<IHttpActionResult> PatchItem(Guid id, [FromBody] JsonPatch.JsonPatchDocument<ListItem> patch)
            {
                var existingItem = _items.FirstOrDefault((item) => item.Id == id);
                if (existingItem == null)
                {
                    await Task.FromResult<IHttpActionResult>(NotFound());
                }

                patch.ApplyUpdatesTo(existingItem);

                return await Task.FromResult<IHttpActionResult>(Ok(existingItem));
            }

            #endregion
        }
    }
}
