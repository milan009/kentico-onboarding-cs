using System;
using System.Threading.Tasks;
using System.Web.Http;
using ListApp.Contracts.Models;

namespace ListApp.Contracts.Interfaces
{
    public interface IDeleteItemService
    {
        Task<IHttpActionResult> DeleteItemAsync(Guid id);
    }
}