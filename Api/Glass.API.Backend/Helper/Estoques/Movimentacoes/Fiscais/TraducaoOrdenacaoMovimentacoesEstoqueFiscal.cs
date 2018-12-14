// <copyright file="TraducaoOrdenacaoMovimentacoesEstoqueFiscal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Estoques.Movimentacoes.Fiscais
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de movimentação do estoque fiscal.
    /// </summary>
    internal class TraducaoOrdenacaoMovimentacaoEstoqueFiscal : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoMovimentacaoEstoqueFiscal"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoMovimentacaoEstoqueFiscal(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "idMovEstoqueFiscal DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}
