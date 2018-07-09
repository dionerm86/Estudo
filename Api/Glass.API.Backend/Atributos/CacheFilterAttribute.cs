// <copyright file="CacheFilterAttribute.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace Glass.API.Backend.Atributos
{
    /// <summary>
    /// Classe com o filtro para controle do cache nas requisições à API.
    /// </summary>
    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response.Headers.CacheControl == null)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue();
            }

            actionExecutedContext.Response.Headers.CacheControl.NoCache = true;
        }
    }
}
