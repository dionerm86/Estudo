// <copyright file="BaseStatusMensagem.cs" company="Sync Softwares">
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
    /// Classe base para respostas HTTP que contenham código e mensagem.
    /// </summary>
    internal abstract class BaseStatusMensagem : IHttpActionResult
    {
        private readonly ApiController apiController;
        private readonly string mensagem;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseStatusMensagem"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        protected BaseStatusMensagem(ApiController apiController, string mensagem)
        {
            this.apiController = apiController;
            this.mensagem = mensagem;
        }

        /// <summary>
        /// Obtém o código de status HTTP que a classe representa.
        /// </summary>
        protected abstract HttpStatusCode Status { get; }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Func<HttpResponseMessage> obterResposta = () =>
            {
                var resposta = this.CriarResposta();
                return this.apiController.Request.CreateResponse(this.Status, resposta);
            };

            return Task.Run(obterResposta, cancellationToken);
        }

        /// <summary>
        /// Cria a resposta que será enviada no retorno do método.
        /// </summary>
        /// <returns>Um objeto com a resposta HTTP.</returns>
        protected virtual MensagemDto CriarResposta()
        {
            return new MensagemDto
            {
                Codigo = (int)this.Status,
                Mensagem = this.mensagem,
            };
        }
    }
}
