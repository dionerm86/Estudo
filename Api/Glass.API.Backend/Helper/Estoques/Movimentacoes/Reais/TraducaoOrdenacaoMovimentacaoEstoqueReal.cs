// <copyright file="TraducaoOrdenacaoMovimentacaoEstoqueReal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Estoques.Movimentacoes.Reais
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de movimentação do estoque real.
    /// </summary>
    internal class TraducaoOrdenacaoMovimentacaoEstoqueReal : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoMovimentacaoEstoqueReal"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoMovimentacaoEstoqueReal(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "idMovEstoque DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}
