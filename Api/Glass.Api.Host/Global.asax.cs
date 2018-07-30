using System.Web.Http;

namespace Glass.Api.Host
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Inicializa o bootstrapper
            new Bootstrapper().Run();


            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Configura as imagens de projeto
            // ImagesConfig.Configure();
        }
    }
}
