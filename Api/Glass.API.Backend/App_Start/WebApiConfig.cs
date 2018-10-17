// <copyright file="WebApiConfig.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Negociacoes;
using System.Configuration;
using System.Net.Http.Formatting;
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

            ConfigurarNegociadorDeConteudo(config);

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

        private static void ConfigurarNegociadorDeConteudo(HttpConfiguration config)
        {
            if (PermitirXml())
            {
                return;
            }

            var formatadorJson = new JsonMediaTypeFormatter();

            config.Services.Replace(
                typeof(IContentNegotiator),
                new JsonContentNegotiator(formatadorJson));
        }

        private static bool PermitirXml()
        {
            var configuracao = ConfigurationManager.AppSettings["permitirXml"];

            bool resultado;
            return !string.IsNullOrEmpty(configuracao)
                && bool.TryParse(configuracao, out resultado)
                && resultado;
        }
    }
}
