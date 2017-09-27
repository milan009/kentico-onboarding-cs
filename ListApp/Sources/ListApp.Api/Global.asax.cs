using System.Web;
using System.Web.Http;

namespace ListApp.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(SerializerConfig.Register);
            GlobalConfiguration.Configure(DependencyResolverConfig.Register);
        }
    }
}
