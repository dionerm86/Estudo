// <copyright file="Global.asax.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Web.Http;

namespace Glass.API.Backend
{
    /// <summary>
    /// Classe que encapsula os métodos de controle do ciclo de vida da aplicação.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Método executado no warm-up da aplicação.
        /// </summary>
        protected void Application_Start()
        {
            new Api.Host.Bootstrapper().Run();

            GlobalConfiguration.Configure(config =>
            {
                WebApiConfig.Register(config);
                GdaConfig.Initialize(config);
                SwaggerConfig.Initialize(config);
            });
        }
    }
}
