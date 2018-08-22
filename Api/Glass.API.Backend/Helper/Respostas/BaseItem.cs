// <copyright file="BaseItem.cs" company="Sync Softwares">
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
    /// Classe que contém uma resposta com um único item no payload.
    /// </summary>
    /// <typeparam name="T">O tipo do item.</typeparam>
    internal abstract class BaseItem<T> : IHttpActionResult
    {
        private readonly ApiController apiController;
        private readonly T item;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseItem{T}"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="item">O item que será retornado.</param>
        protected BaseItem(ApiController apiController, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.apiController = apiController;
            this.item = item;
        }

        /// <summary>
        /// Obtém o código de status HTTP para a resposta.
        /// </summary>
        protected abstract HttpStatusCode CodigoStatus { get; }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<HttpResponseMessage> obterResposta = () =>
            {
                return this.apiController.Request.CreateResponse(this.CodigoStatus, this.item);
            };

            return Task.Run(obterResposta, cancellationToken);
        }
    }
}
