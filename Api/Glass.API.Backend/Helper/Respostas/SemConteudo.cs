// <copyright file="SemConteudo.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula a resposta NoContent (204) do HTTP.
    /// </summary>
    internal class SemConteudo : IHttpActionResult
    {
        private static readonly object VAZIO = new { };
        private readonly ApiController apiController;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="SemConteudo"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        public SemConteudo(ApiController apiController)
        {
            this.apiController = apiController;
        }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<HttpResponseMessage> obterResposta = () =>
            {
                return this.apiController.Request.CreateResponse(HttpStatusCode.NoContent, VAZIO);
            };

            return Task.Run(obterResposta, cancellationToken);
        }
    }
}
