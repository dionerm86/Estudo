// <copyright file="Item.cs" company="Sync Softwares">
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
    internal class Item<T> : IHttpActionResult
    {
        private readonly ApiController apiController;
        private readonly T item;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Item{T}"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="item">O item que será retornado.</param>
        public Item(ApiController apiController, T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            this.apiController = apiController;
            this.item = item;
        }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<HttpResponseMessage> obterResposta = () =>
            {
                return this.apiController.Request.CreateResponse(HttpStatusCode.OK, this.item);
            };

            return Task.Run(obterResposta, cancellationToken);
        }
    }
}
