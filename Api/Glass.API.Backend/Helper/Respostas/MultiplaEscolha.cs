// <copyright file="MultiplaEscolha.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Net;
using System.Text;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula a resposta MultipleChoices (300) do HTTP.
    /// </summary>
    internal class MultiplaEscolha : BaseStatusMensagem
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MultiplaEscolha"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        public MultiplaEscolha(ApiController apiController, string mensagem)
            : base(apiController, mensagem)
        {
        }

        /// <inheritdoc/>
        protected override HttpStatusCode Status
        {
            get { return HttpStatusCode.MultipleChoices; }
        }
    }
}
