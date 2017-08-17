using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
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
            private static List<ListItem> items = new List<ListItem>();

            static ItemsController()
            {
                items.Add(new ListItem{Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Text = "Stretch correctly"});
                items.Add(new ListItem{Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a coffey"});
                items.Add(new ListItem{Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world"});
            }
            
            [Route]
            [HttpGet]
            public IEnumerable<ListItem> GetItems()
            {
                return items;
            }        

            [Route("{id}")]
            [HttpGet]
            public IHttpActionResult GetItem(string id)
            {
                if (!Guid.TryParse(id, out Guid guid))
                {
                    return BadRequest("Specified ID is not a valid GUID");
                }

                var theItem = items.FirstOrDefault((item) => item.Id == guid);
                if (theItem != null)
                {
                    return Ok(theItem);
                }

                return NotFound();
            }

            [Route]
            [HttpPost]
            public IHttpActionResult PostItem([FromBody]string newItemText)
            {
                var createdItem = new ListItem{Id = Guid.NewGuid(), Text = newItemText};

                items.Add(createdItem);

                return Created($"/items/{createdItem.Id}", createdItem);
            }

            [Route("{id}")]
            [HttpPut]
            public IHttpActionResult PutItem(string id, ListItem newItem)
            {
                if (!Guid.TryParse(id, out Guid guid))
                {
                    return BadRequest("Specified ID is not a valid GUID");
                }

                var existingItem = items.FirstOrDefault((item) => item.Id == guid);
                if (existingItem == null)
                {
                    items.Add(newItem);
                    return Created($"/items/{id}", newItem);
                }

                existingItem.Text = newItem.Text;

                return Ok(existingItem);
            }
        }
    }
}
