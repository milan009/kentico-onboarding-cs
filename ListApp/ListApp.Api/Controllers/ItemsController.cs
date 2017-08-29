using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using ListApp.Api.Models;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers
{
    namespace V1
    {
        [ApiVersion("1.0")]
        [RoutePrefix("api/v{version:apiVersion}/items")]
        public class ItemsController : ApiController
        {
            private static readonly List<ListItem> Items = new List<ListItem>();
            private readonly Func<Guid> _idGenerator;

            static ItemsController()
            {
                Items.Add(new ListItem{Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Text = "Stretch correctly"});
                Items.Add(new ListItem{Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a coffey"});
                Items.Add(new ListItem{Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world"});
            }

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
            public async Task<IHttpActionResult> GetItem(string id)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    if (!Guid.TryParse(id, out Guid guid))
                    {
                        return BadRequest("Specified ID is not a valid GUID");
                    }

                    var theItem = Items.FirstOrDefault((item) => item.Id == guid);
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
                    var createdItem = new ListItem {Id = _idGenerator(), Text = newItemText};

                    Items.Add(createdItem);

                    return Created($"/items/{createdItem.Id}", createdItem);
                });
            }

            [Route("{id}")]
            [HttpPut]
            public async Task<IHttpActionResult> PutItem(string id, [FromBody] ListItem newItem)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    if (!Guid.TryParse(id, out Guid guid))
                    {
                        return BadRequest("Specified ID is not a valid GUID");
                    }

                    var existingItem = Items.FirstOrDefault((item) => item.Id == guid);
                    if (existingItem == null)
                    {
                        Items.Add(newItem);
                        return Created($"/items/{id}", newItem);
                    }

                    existingItem = newItem;

                    return Ok(existingItem);
                });
            }

            [Route("{id}")]
            [HttpDelete]
            public async Task<IHttpActionResult> DeleteItem(string id)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    if (!Guid.TryParse(id, out Guid guid))
                    {
                        return BadRequest("Specified ID is not a valid GUID");
                    }

                    var existingItem = Items.FirstOrDefault((item) => item.Id == guid);
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
            public async Task<IHttpActionResult> PatchItem(string id, [FromBody] JsonPatch.JsonPatchDocument<ListItem> patch)
            {
                return await Task<IHttpActionResult>.Factory.StartNew(() =>
                {
                    if (patch == null)
                    {
                        return BadRequest("Missing patch request body");
                    }

                    if (!Guid.TryParse(id, out Guid guid))
                    {
                        return BadRequest("Specified ID is not a valid GUID");
                    }

                    var existingItem = Items.FirstOrDefault((item) => item.Id == guid);
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
