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
            private readonly IRepository<ListItem, Guid> _itemsRepository;
            private readonly Func<Guid> _idGenerator;

            // Paramless constructor will be using Guid.NewGuid to generate GUIDs
            // and an instance of ListItemRepository()
            public ItemsController() : this(Guid.NewGuid, new ListItemRepository()) { }

            public ItemsController(Func<Guid> idGenerator, IRepository<ListItem, Guid> itemsRepository)
            {
                _itemsRepository = itemsRepository;
                _idGenerator = idGenerator;
            }

            #region HTTP verbs implementations

            [Route]
            [HttpGet]
            public async Task<IEnumerable<ListItem>> GetItemsAsync()
            {
                return await Task.FromResult(_itemsRepository.GetAll());
            }

            [Route("{id}")]
            [HttpGet]
            public async Task<IHttpActionResult> GetItemAsync(Guid id)
            {
                var theItem = _itemsRepository.Get(id);
                if (theItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }

                return await Task.FromResult<IHttpActionResult>(Ok(theItem));
            }

            [Route]
            [HttpPost]
            public async Task<IHttpActionResult> PostItemAsync([FromBody]string newItemText)
            {
                var createdItem = new ListItem{Id = _idGenerator(), Text = newItemText};

                try
                {
                    _itemsRepository.Add(createdItem.Id, createdItem);
                }
                catch (DuplicateKeyException e)
                {
                    if (_idGenerator == Guid.NewGuid)
                    {
                        return await Task.FromResult<IHttpActionResult>(InternalServerError(
                            new DuplicateKeyException(
                                createdItem.Id, "The generated Guid conflicts with already existing one." +
                                                "The chance of this happening is so slim that you are " +
                                                "more likely to be hit by a meteorite. " +
                                                "Count yourself lucky!")));
                    }

                    return await Task.FromResult<IHttpActionResult>(InternalServerError(e));

                }
                    
                return await Task.FromResult<IHttpActionResult>(Created($"/items/{createdItem.Id}", createdItem));
            }

            [Route]
            [HttpPut]
            [PutCollectionActionFilter]
            public async Task<IHttpActionResult> PutItemsCollectionAsync([FromBody] IEnumerable<ListItem> items)
            {
                _itemsRepository.Clear();
                var listItems = items as IList<ListItem> ?? items.ToList();

                // The uniqueness of GUIDs is tested in the filter, no need to check here
                foreach (var item in listItems)
                {
                    _itemsRepository.Add(item.Id, item);
                }

                return await Task.FromResult<IHttpActionResult>(Created("/items", _itemsRepository.GetAll()));
            }

            [Route("{id}")]
            [HttpPut]
            [PutGuidConsistencyActionFilter]
            public async Task<IHttpActionResult> PutItemAsync(Guid id, [FromBody] ListItem newItem)
            {
                if (_itemsRepository.GetKeys().Contains(id))
                {
                    _itemsRepository.Delete(id);
                }

                _itemsRepository.Add(id, newItem);

                return await Task.FromResult<IHttpActionResult>(Created($"/items/{id}", newItem));
            }

            [Route]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItemsAsync([FromBody]IEnumerable<Guid> idsToDelete)
            {
                var toDelete = idsToDelete as IList<Guid> ?? idsToDelete.ToList();

                if (!toDelete.All(idToDelete => _itemsRepository.GetKeys().Contains(idToDelete)))
                    return await Task.FromResult<IHttpActionResult>(Content(HttpStatusCode.NotFound,
                        "One or more of IDs specfied for deletion has not been found."));

                foreach (var id in toDelete)
                {
                    _itemsRepository.Delete(id);
                }

                return await Task.FromResult<IHttpActionResult>(Ok());
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItemAsync(Guid id)
            {
                try
                {
                    _itemsRepository.Delete(id);
                }
                catch (KeyNotFoundException e)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }

                return await Task.FromResult<IHttpActionResult>(Ok());
            }

            /// <summary>
            /// Applies a Json patch to a ListItem specified by given GUID. The GUID is taken from the URL,
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
                var existingItem = _itemsRepository.Get(id);
                if (existingItem == null)
                {
                    return await Task.FromResult<IHttpActionResult>(NotFound());
                }
                
                patch.ApplyUpdatesTo(existingItem);

                _itemsRepository.Delete(id);
                _itemsRepository.Add(id, existingItem);
               
                return await Task.FromResult<IHttpActionResult>(Ok(existingItem));
            }

            #endregion
        }
    }
}
