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
        [Route("api/v{version:apiVersion}/items")]
        public class ItemsController : ApiController
        {
            private static List<ListItem> items = new List<ListItem>();

            static ItemsController()
            {
                items.Add(new ListItem{Id = "0", Text = "Stretch correctly"});
                items.Add(new ListItem{Id = "1", Text = "Make a coffey"});
                items.Add(new ListItem{Id = "2", Text = "Take over the world"});
            }

            public IEnumerable<ListItem> GetItems()
            {
                return items;
            }        

            [Route("api/v{version:apiVersion}/items/{id}")]
            public IHttpActionResult GetItem(string id)
            {
                var theItem = items.First((item) => item.Id == id);
                if (theItem != null)
                {
                    return Ok(theItem);
                }

                return NotFound();
            }

            public IHttpActionResult PostItem(ListItem newItem)
            {
                var checkedItem = items.FirstOrDefault((item) => item.Id == newItem.Id);

                if (checkedItem != null)
                {
                    return Conflict();
                }

                items.Add(newItem);

                return Created($"/items/{newItem.Id}", newItem);
            }

            [Route("api/v{version:apiVersion}/items/{id}")]
            public IHttpActionResult PutItem(string id, ListItem newItem)
            {
                var checkedItem = items.First((item) => item.Id == id);

                if (checkedItem == null)
                {
                    items.Add(newItem);
                    return Created($"/items/{id}", newItem);
                }

                checkedItem.Text = newItem.Text;

                return Ok(checkedItem);
            }
        }
    }
}
