// <copyright file="Criado.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.Net;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que encapsula o status Created (201) do HTTP.
    /// </summary>
    /// <typeparam name="T">O tipo do identificador do item.</typeparam>
    internal class Criado<T> : BaseStatusMensagem
    {
        private readonly T identificadorItem;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Criado{T}"/>.
        /// </summary>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="mensagem">A mensagem que será exibida na tela.</param>
        /// <param name="identificadorItem">O identificador do item que foi criado.</param>
        public Criado(ApiController apiController, string mensagem, T identificadorItem)
            : base(apiController, mensagem)
        {
            this.identificadorItem = identificadorItem;
        }

        /// <inheritdoc/>
        protected override HttpStatusCode Status
        {
            get { return HttpStatusCode.Created; }
        }

        /// <inheritdoc/>
        protected override MensagemDto CriarResposta()
        {
            var respostaBase = base.CriarResposta();

            return new CriadoDto<T>
            {
                Codigo = respostaBase.Codigo,
                Mensagem = respostaBase.Mensagem,
                Id = this.identificadorItem,
            };
        }
    }
}
