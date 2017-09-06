using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Filters;
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
            // Currently used to simulate a way to store ListItems
            private static List<ListItem> _items;
            private readonly Func<Guid> _idGenerator;

            // Paramless constructor will be using Guid.NewGuid to generate GUIDs
            public ItemsController() : this(Guid.NewGuid) { }

            public ItemsController(Func<Guid> idGenerator)
            {
                _idGenerator = idGenerator;
                InitializeItems();
            }

            // Initializes the static list of ListItems to default mock value
            private void InitializeItems()
            {
                _items = new List<ListItem>(Utils.Constants.MockListItems);
            }

            #region HTTP verbs implementations

            [Route]
            [HttpGet]
            public async Task<IEnumerable<ListItem>> GetItemsAsync()
            {
                return await Task.FromResult<IEnumerable<ListItem>>(_items);
            }

            [Route("{id}")]
            [HttpGet]
            public async Task<IHttpActionResult> GetItemAsync(Guid id)
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
            public async Task<IHttpActionResult> PostItemAsync([FromBody]string newItemText)
            {
                var createdItem = new ListItem{Id = _idGenerator(), Text = newItemText};

                _items.Add(createdItem);
                    
                return await Task.FromResult<IHttpActionResult>(Created($"/items/{createdItem.Id}", createdItem));
            }

            [Route]
            [HttpPut]
            [PutCollectionActionFilter]
            public async Task<IHttpActionResult> PutItemsCollectionAsync([FromBody] IEnumerable<ListItem> items)
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
            public async Task<IHttpActionResult> PutItemAsync(Guid id, [FromBody] ListItem newItem)
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

            [Route]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItemsAsync([FromBody]IEnumerable<Guid> idsToDelete)
            {
                var toDelete = idsToDelete as IList<Guid> ?? idsToDelete.ToList();

                if (toDelete.All(idToDelete => _items.Exists(item => item.Id == idToDelete)))
                {
                    _items.RemoveAll(item => toDelete.Contains(item.Id));
                    return await Task.FromResult<IHttpActionResult>(Ok());
                }

                return await Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.NotFound,
                    "One or more of IDs specfied for deletion has not been found."));
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItemAsync(Guid id)
            {
                var existingItem = _items.FirstOrDefault((item) => item.Id == id);
                if (existingItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }

                _items.Remove(existingItem);

                return await Task.FromResult<IHttpActionResult>(Ok());
            }

            /// <summary>
            /// Applies a Json patch to á ListItem specified by given GUID. The GUID is taken from the URL,
            /// the patch object from the body of the request.
            /// Only "replace" operation is allowed on a given resource, as all its props are required. 
            /// </summary>
            /// <param name="id">The GUID specifying the resource to patch</param>
            /// <param name="patch">The Json patch object</param>
            /// <returns>404 if target resource is not found, 200 with modified item in the body if resource is found and patched succesfully</returns>
            [Route("{id}")]
            [HttpPatch]
            [PatchSingleResourceActionFilter]
            public async Task<IHttpActionResult> PatchItemAsync(Guid id, [FromBody] JsonPatch.JsonPatchDocument<ListItem> patch)
            {
                var existingItem = _items.FirstOrDefault((item) => item.Id == id);
                if (existingItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }
                
                patch.ApplyUpdatesTo(existingItem);
               
                return await Task.FromResult<IHttpActionResult>(Ok(existingItem));
            }

            #endregion
        }
    }
}
