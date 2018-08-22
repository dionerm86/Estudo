// <copyright file="ErroInternoServidor.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Net;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula o status InternalServerError (500) do HTTP.
    /// </summary>
    internal class ErroInternoServidor : ErroValidacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ErroInternoServidor"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        /// <param name="erro">O erro que ocorreu.</param>
        public ErroInternoServidor(ApiController apiController, string mensagem, Exception erro)
            : base(apiController, mensagem, erro)
        {
        }

        /// <inheritdoc/>
        protected override HttpStatusCode Status
        {
            get { return HttpStatusCode.InternalServerError; }
        }
    }
}
