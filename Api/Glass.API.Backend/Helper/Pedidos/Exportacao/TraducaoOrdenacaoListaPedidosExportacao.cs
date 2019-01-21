// <copyright file="TraducaoOrdenacaoListaPedidosExportacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Pedidos.Exportacao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de pedidos para exportação.
    /// </summary>
    internal class TraducaoOrdenacaoListaPedidosExportacao : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPedidosExportacao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na listagem de pedidos para exportação.</param>
        public TraducaoOrdenacaoListaPedidosExportacao(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdPedido"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}