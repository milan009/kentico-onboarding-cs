using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace ListApp.Api
{
    internal static class SerializerConfig
    {
        internal static void Register(HttpConfiguration config)
        {
            config
                .Formatters
                .JsonFormatter
                .SerializerSettings
                .ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}