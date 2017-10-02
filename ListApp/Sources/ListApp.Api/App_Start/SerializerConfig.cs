using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace ListApp.Api
{
    public static class SerializerConfig
    {
        public static void Register(HttpConfiguration config) => 
            config
                .Formatters
                .JsonFormatter 
                .SerializerSettings
                .ContractResolver = new CamelCasePropertyNamesContractResolver();
    }
}
