// <copyright file="TraducaoOrdenacaoListaPedidosConferencia.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.PedidosConferencia
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de pedidos em conferência.
    /// </summary>
    internal class TraducaoOrdenacaoListaPedidosConferencia : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPedidosConferencia"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPedidosConferencia(string ordenacao)
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
                case "id":
                    return "IdPedido";

                case "nomeconferente":
                    return "Conferente";

                case "nomecliente":
                    return "NomeCliente";

                case "totalpedidocomercial":
                    return "TotalPedido";

                case "totalpedidoconferencia":
                    return "Total";

                case "datacadastroconferencia":
                    return "DataEspelho";

                case "datafinalizacaoconferencia":
                    return "DataConf";

                case "totalm2":
                    return "TotM";

                case "dataentregapedidocomercial":
                    return "DataEntrega";

                case "dataentregafabrica":
                    return "DataFabrica";

                case "nomeloja":
                case "peso":
                case "situacao":
                case "situacaocnc":
                case "pedidoconferido":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
