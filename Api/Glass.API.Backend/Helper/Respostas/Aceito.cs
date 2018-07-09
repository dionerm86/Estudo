// <copyright file="Aceito.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Net;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula a resposta Accepted (202) do HTTP.
    /// </summary>
    internal class Aceito : BaseStatusMensagem
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Aceito"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        public Aceito(ApiController apiController, string mensagem)
            : base(apiController, mensagem)
        {
        }

        /// <inheritdoc/>
        protected override HttpStatusCode Status
        {
            get { return HttpStatusCode.Accepted; }
        }
    }
}
