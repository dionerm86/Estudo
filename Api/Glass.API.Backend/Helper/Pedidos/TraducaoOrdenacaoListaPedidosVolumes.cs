// <copyright file="TraducaoOrdenacaoListaPedidosVolumes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Pedidos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de pedidos.
    /// </summary>
    internal class TraducaoOrdenacaoListaPedidosVolumes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPedidosVolumes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPedidosVolumes(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdPedido DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "pedido":
                    return "IdPedido";

                case "cliente":
                    return "IdCli";

                case "loja":
                    return "NomeLoja";

                case "funcionario":
                    return "NomeFunc";

                case "rota":
                    return "CodRota";

                case "dataentrega":
                    return "DataEntrega";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
