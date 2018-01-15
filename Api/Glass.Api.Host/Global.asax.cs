using System.Web.Http;

namespace Glass.Api.Host
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //Inicializa o bootstrapper
            new Bootstrapper().Run();
        }
    }
}
