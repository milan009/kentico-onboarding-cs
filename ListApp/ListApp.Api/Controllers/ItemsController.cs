using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ListApp.Api.Models;
using Microsoft.Web.Http;

namespace ListApp.Api.Controllers
{
    namespace V1
    {
        [ApiVersion("1.0")]
        [Route("api/v{version:apiVersion}/list")]

        public class ItemsController : ApiController
        {
            
        }
    }
}
