// <copyright file="Item.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Net;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que contém uma resposta com um único item no payload.
    /// Status de resposta "OK".
    /// </summary>
    /// <typeparam name="T">O tipo do item.</typeparam>
    internal class Item<T> : BaseItem<T>
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Item{T}"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="item">O item que será retornado.</param>
        public Item(ApiController apiController, T item)
            : base(apiController, item)
        {
        }

        /// <inheritdoc/>
        protected override HttpStatusCode CodigoStatus
        {
            get { return HttpStatusCode.OK; }
        }
    }
}
