using System;
using System.Web.Http;
using System.Web.Http.Routing;
using ListApp.Contracts.Interfaces;
using ListApp.Contracts.Models;
using ListApp.Repositories;

using ListApp.Api.Utils;
using Microsoft.Practices.Unity;
using Microsoft.Web.Http.Routing;
using Newtonsoft.Json.Serialization;

namespace ListApp.Api
{
    public static class SerializerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
