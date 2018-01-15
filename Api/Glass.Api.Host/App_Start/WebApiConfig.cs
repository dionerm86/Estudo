using System.Net.Http;
using System.Web.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http.Cors;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace Glass.Api.Host
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Colosoft.Web.Http.CorsHttpConfigurationExtensions.EnableCors(config, new Colosoft.Web.Http.Cors.EnableCorsAttribute("*", "*", "*"));

            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            Colosoft.Web.Http.ModelBinding.MultiPostParameterBinding.GlobalJsonSerializerSettingsGetter =
                () => GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings;

            GlobalConfiguration.Configuration.ParameterBindingRules.Insert(0, Colosoft.Web.Http.ModelBinding.MultiPostParameterBinding.CreateBindingForMarkedParameters);

            config.Services.Insert(typeof(System.Web.Http.ModelBinding.ModelBinderProvider), 0,
                new Colosoft.Web.Http.ModelBinding.NativeNullableTypesModelBinderProvider(Glass.Globalizacao.Cultura.CulturaSistema));
            config.Services.Add(typeof(System.Web.Http.ModelBinding.ModelBinderProvider), new System.Web.Http.ModelBinding.Binders.ArrayModelBinderProvider());

            var formatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            formatter.SerializerSettings.ContractResolver = new CustomResolver();

            config.Formatters.Add(new BrowserJsonFormatter());

            // Configura a Web API para usar somente autenticação com token bearer
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "seguranca",
                routeTemplate: "Seguranca/{controller}/{action}/{id}",
                defaults: new
                {
                    area = "Seguranca",
                    controller = "Index",
                    action = "Get",
                    id = RouteParameter.Optional,
                    namespaces = new string[] { "Glass.Api.Host.Areas.Seguranca.Controllers" }
                }
            );

            config.Routes.MapHttpRoute(
                name: "graficos",
                routeTemplate: "Graficos/{controller}/{action}/{id}",
                defaults: new
                {
                    area = "Graficos",
                    controller = "Index",
                    action = "Get",
                    id = RouteParameter.Optional,
                    namespaces = new string[] { "Glass.Api.Host.Areas.Graficos.Controllers" }
                }
            );

            config.Routes.MapHttpRoute(
                name: "app",
                routeTemplate: "App/{controller}/{action}/{id}",
                defaults: new
                {
                    area = "App",
                    controller = "Index",
                    action = "Get",
                    id = RouteParameter.Optional,
                    namespaces = new string[] { "Glass.Api.Host.Areas.App.Controllers" }
                }
            );

            config.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    controller = "Index",
                    action = "Get",
                    namespaces = new string[] { "Glass.Api.Host.Controllers" }
                }
            );

            // Enforce HTTPS
           config.Filters.Add(new Filters.RequireHttpsAttribute());
        }

        /// <summary>
        /// Implementação do Resolver customizado para o contrato dos tipos para JSON.
        /// </summary>
        class CustomResolver : DefaultContractResolver
        {
            #region Métodos Protegidos

            /// <summary>
            /// Cria o contrato para o tipo informado.
            /// </summary>
            /// <param name="objectType"></param>
            /// <returns></returns>
            protected override JsonContract CreateContract(Type objectType)
            {
                JsonContract contract = base.CreateContract(objectType);

                if (typeof(Colosoft.IMessageFormattable).IsAssignableFrom(objectType))
                    contract.Converter = new Colosoft.Web.Json.Converters.MessageFormattableConverter();

                return contract;
            }

            #endregion
        }

        /// <summary>
        /// Implementação do formatador de Json.
        /// </summary>
        public class BrowserJsonFormatter : System.Net.Http.Formatting.JsonMediaTypeFormatter
        {
            #region Constructors

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            public BrowserJsonFormatter()
            {
                this.SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
                this.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                this.SupportedEncodings.Insert(0, System.Text.Encoding.Default);
                //this.SupportedEncodings.Clear();
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Define os cabeçalhos de conteúdo padrão.
            /// </summary>
            /// <param name="type"></param>
            /// <param name="headers"></param>
            /// <param name="mediaType"></param>
            public override void SetDefaultContentHeaders(System.Type type, System.Net.Http.Headers.HttpContentHeaders headers, System.Net.Http.Headers.MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            }

            #endregion
        }
    }
}
