// <copyright file="TraducaoOrdenacaoListaNaturezasOperacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Cfops.NaturezasOperacao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de naturezas de operação.
    /// </summary>
    internal class TraducaoOrdenacaoListaNaturezasOperacao : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaNaturezasOperacao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaNaturezasOperacao(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "CodInterno ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}
