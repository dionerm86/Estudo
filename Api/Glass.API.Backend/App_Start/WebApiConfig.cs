// <copyright file="WebApiConfig.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Glass.API.Backend
{
    /// <summary>
    /// Classe com a configuração para a API.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Inicia as configurações da API no warm-up da aplicação.
        /// </summary>
        /// <param name="config">Configuração HTTP da aplicação.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var authorizedUris = ConfigurationManager.AppSettings["authorizedUris"];
            config.EnableCors(new EnableCorsAttribute(authorizedUris, "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            var defaults = new
            {
                version = "v1",
                id = RouteParameter.Optional,
                action = RouteParameter.Optional,
            };

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{version}/{controller}/{id}/{action}",
                defaults: defaults);
        }
    }
}
