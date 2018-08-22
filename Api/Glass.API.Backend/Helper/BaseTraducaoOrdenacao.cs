// <copyright file="BaseTraducaoOrdenacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Classe base para a tradução de campos para ordenação.
    /// </summary>
    internal abstract class BaseTraducaoOrdenacao : ITraducaoOrdenacao
    {
        private readonly string[] dadosOrdenacao;
        private readonly Lazy<string> ordenacaoTraduzida;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseTraducaoOrdenacao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        protected BaseTraducaoOrdenacao(string ordenacao)
        {
            this.dadosOrdenacao = string.IsNullOrWhiteSpace(ordenacao)
                ? new string[0]
                : ordenacao.Split(' ');

            this.ordenacaoTraduzida = new Lazy<string>(() => this.TraduzirOrdenacao());
        }

        /// <summary>
        /// Obtém a ordenação padrão para o tradutor.
        /// </summary>
        protected abstract string OrdenacaoPadrao { get; }

        /// <summary>
        /// Retorna o campo traduzido para a ordenação da tela.
        /// </summary>
        /// <returns>O campo traduzido, se possível.</returns>
        public string ObterTraducaoOrdenacao()
        {
            return this.ordenacaoTraduzida.Value;
        }

        /// <summary>
        /// Realiza a tradução do campo informado na tela.
        /// </summary>
        /// <param name="campo">O capmo informado na tela.</param>
        /// <returns>A tradução da ordenação.</returns>
        protected abstract string TraduzirCampo(string campo);

        private string TraduzirOrdenacao()
        {
            if (this.dadosOrdenacao.Length == 0)
            {
                return this.OrdenacaoPadrao;
            }

            var traducao = this.TraduzirCampo(this.dadosOrdenacao[0])
                ?? this.OrdenacaoPadrao;

            return traducao != this.OrdenacaoPadrao
                ? this.IncluiDirecao(traducao)
                : traducao;
        }

        private string IncluiDirecao(string campo)
        {
            string direcao = this.dadosOrdenacao.Length > 1
                ? string.Format(" {0}", this.dadosOrdenacao[1])
                : string.Empty;

            return campo + direcao;
        }
    }
}
