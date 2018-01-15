using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Glass.Api.Host.Filters
{
    /// <summary>
    /// Atributo para o filtro que será usado para forçar a utilização
    /// do https.
    /// </summary>
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        #region Propriedades

        /// <summary>
        /// Porta.
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public RequireHttpsAttribute()
        {
            Port = 443;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Método acionado na autorização da requisição.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                var response = new HttpResponseMessage();

                if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
                {
                    var uri = new UriBuilder(request.RequestUri);
                    uri.Scheme = Uri.UriSchemeHttps;
                    uri.Port = this.Port;

                    response.StatusCode = HttpStatusCode.Found;
                    response.Headers.Location = uri.Uri;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.Forbidden;
                }

                actionContext.Response = response;
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }

        #endregion
    }
}