// <copyright file="TraducaoOrdenacaoExtratoMovimentacoesChapa.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.MateriaPrima.Extrato.MovimentacaoChapa
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista extrato de movimentação de chapa.
    /// </summary>
    internal class TraducaoOrdenacaoExtratoMovimentacoesChapa : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoExtratoMovimentacoesChapa"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoExtratoMovimentacoesChapa(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdCorVidro ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}