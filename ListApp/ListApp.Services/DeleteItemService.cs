using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;

namespace ListApp.Api.Services
{
    class DeleteItemService : IDeleteItemService
    {
        private readonly IRepository _repository;

        public DeleteItemService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IHttpActionResult> DeleteItemAsync(Guid id)
        {
            ListItem deletedItem;
            var request = (HttpRequestMessage) HttpContext.Current.Items["MS_HttpRequestMessage"];
            try
            {
                deletedItem = await _repository.DeleteAsync(id);
            }
            catch (Exception e)
            {
                return new InternalServerErrorResult(request);
            }

            if (deletedItem == null)
            {
                return new NotFoundResult(request);
            }

            return new OkNegotiatedContentResult<ListItem>(deletedItem, new DefaultContentNegotiator(), request, );

            throw new NotImplementedException();
        }
    }
}
