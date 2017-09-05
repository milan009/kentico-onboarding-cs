using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Api.Filters;
using Microsoft.Web.Http;
using ListApp.Api.Models;
using ListApp.Api.Repositories;

namespace ListApp.Api.Controllers
{
    namespace V1
    {
        [ApiVersion("1.0")]
        [RoutePrefix("api/v{version:apiVersion}/items")]
        public class ItemsController : ApiController
        {
            private readonly IRepository<ListItem, Guid> _items;
            private readonly Func<Guid> _idGenerator;

            // Paramless constructor will be using Guid.NewGuid to generate GUIDs
            public ItemsController() : this(Guid.NewGuid) { }

            public ItemsController(Func<Guid> idGenerator)
            {
                _idGenerator = idGenerator;
            }

            #region HTTP verbs implementations

            [Route]
            [HttpGet]
            public async Task<IEnumerable<ListItem>> GetItems()
            {
                return await Task.FromResult(_items.GetAll());
            }

            [Route("{id}")]
            [HttpGet]
            public async Task<IHttpActionResult> GetItem(Guid id)
            {
                var theItem = _items.Get(id);
                if (theItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }

                return await Task.FromResult<IHttpActionResult>(Ok(theItem));
            }

            [Route]
            [HttpPost]
            public async Task<IHttpActionResult> PostItem([FromBody]string newItemText)
            {
                var createdItem = new ListItem{Id = _idGenerator(), Text = newItemText};

                try
                {
                    _items.Add(createdItem.Id, createdItem);
                }
                catch (DuplicateKeyException e)
                {
                    return await Task.FromResult<IHttpActionResult>(Conflict());
                }
                    
                return await Task.FromResult<IHttpActionResult>(Created($"/items/{createdItem.Id}", createdItem));
            }

            [Route]
            [HttpPut]
            [PutCollectionActionFilter]
            public async Task<IHttpActionResult> PutItemsCollection([FromBody] IEnumerable<ListItem> items)
            {
                _items.Clear();
                var listItems = items as IList<ListItem> ?? items.ToList();

                // The uniqueness of GUIDs is tested in the filter, no need to check here
                foreach (var item in listItems)
                {
                    _items.Add(item.Id, item);
                }

                return await Task.FromResult<IHttpActionResult>(Created("/items", _items));
            }

            [Route("{id}")]
            [HttpPut]
            [PutGuidConsistencyActionFilter]
            public async Task<IHttpActionResult> PutItem(Guid id, [FromBody] ListItem newItem)
            {
                if (_items.GetKeys().Contains(id))
                {
                    _items.Delete(id);
                }

                _items.Add(id, newItem);

                return await Task.FromResult<IHttpActionResult>(Created($"/items/{id}", newItem));
            }

            [Route]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItems([FromBody]IEnumerable<Guid> idsToDelete)
            {
                var toDelete = idsToDelete as IList<Guid> ?? idsToDelete.ToList();

                if (toDelete.All(idToDelete => _items.GetKeys().Contains(idToDelete)))
                {
                    foreach (var id in toDelete)
                    {
                        _items.Delete(id);
                    }
                    return await Task.FromResult<IHttpActionResult>(Ok());
                }

                return await Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.NotFound,
                    "One or more of IDs specfied for deletion has not been found."));
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItem(Guid id)
            {
                try
                {
                    _items.Delete(id);
                }
                catch (KeyNotFoundException e)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }

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
            public async Task<IHttpActionResult> PatchItem(Guid id, [FromBody] JsonPatch.JsonPatchDocument<ListItem> patch)
            {
                var existingItem = _items.Get(id);
                if (existingItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }
                
                patch.ApplyUpdatesTo(existingItem);

                _items.Delete(id);
                _items.Add(id, existingItem);
               
                return await Task.FromResult<IHttpActionResult>(Ok(existingItem));
            }

            #endregion
        }
    }
}
