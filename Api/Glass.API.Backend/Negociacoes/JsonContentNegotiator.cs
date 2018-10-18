// <copyright file="JsonContentNegotiator.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Glass.API.Backend.Negociacoes
{
    /// <summary>
    /// Classe que altera as negociações para que apenas JSON seja aceito na API.
    /// </summary>
    public class JsonContentNegotiator : IContentNegotiator
    {
        private readonly JsonMediaTypeFormatter formatadorJson;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="JsonContentNegotiator"/>.
        /// </summary>
        /// <param name="formatadorJson">Formatador para a resposta JSON.</param>
        public JsonContentNegotiator(JsonMediaTypeFormatter formatadorJson)
        {
            this.formatadorJson = formatadorJson;
        }

        /// <inheritdoc/>
        public ContentNegotiationResult Negotiate(
            Type type,
            HttpRequestMessage request,
            IEnumerable<MediaTypeFormatter> formatters)
        {
            MediaTypeFormatter formatador = null;

            if (request.RequestUri.ToString().ToLower().Contains("swagger"))
            {
                formatador = formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            }

            return new ContentNegotiationResult(
                formatador ?? this.formatadorJson,
                new MediaTypeHeaderValue("application/json"));
        }
    }
}
