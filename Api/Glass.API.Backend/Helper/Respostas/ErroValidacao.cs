// <copyright file="ErroValidacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Net;
using System.Text;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula a resposta BadRequest (400) do HTTP.
    /// </summary>
    internal class ErroValidacao : BaseStatusMensagem
    {
        private readonly Exception erro;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ErroValidacao"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        /// <param name="erro">O erro de validação que ocorreu.</param>
        public ErroValidacao(ApiController apiController, string mensagem, Exception erro)
            : base(apiController, mensagem)
        {
            this.erro = erro;
        }

        /// <inheritdoc/>
        protected override HttpStatusCode Status
        {
            get { return HttpStatusCode.BadRequest; }
        }

        /// <inheritdoc/>
        protected override MensagemDto CriarResposta()
        {
            var resposta = base.CriarResposta();
            resposta.Mensagem = this.FormatarMensagem(resposta.Mensagem);

            return resposta;
        }

        private string FormatarMensagem(string mensagem)
        {
            var mensagemFormatada = new StringBuilder();
            mensagemFormatada.Append(mensagem);

            if (this.erro != null)
            {
                mensagemFormatada.Append(" Erro: ");
                mensagemFormatada.Append(this.erro.Message);
            }

            return mensagemFormatada.ToString();
        }
    }
}
