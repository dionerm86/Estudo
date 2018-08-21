// <copyright file="NaoEncontrado.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Net;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula a resposta NotFound (404) do HTTP.
    /// </summary>
    internal class NaoEncontrado : BaseStatusMensagem
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="NaoEncontrado"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        public NaoEncontrado(ApiController apiController, string mensagem)
            : base(apiController, mensagem)
        {
        }

        /// <inheritdoc/>
        protected override HttpStatusCode Status
        {
            get { return HttpStatusCode.NotFound; }
        }
    }
}
