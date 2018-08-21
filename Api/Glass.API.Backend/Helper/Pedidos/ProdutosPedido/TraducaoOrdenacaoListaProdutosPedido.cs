// <copyright file="TraducaoOrdenacaoListaProdutosPedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Pedidos.ProdutosPedido
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de produtos do pedido.
    /// </summary>
    internal class TraducaoOrdenacaoListaProdutosPedido : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaProdutosPedido"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        internal TraducaoOrdenacaoListaProdutosPedido(string ordenacao)
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

                case "codigopedidocliente":
                    return "CodCliente";

                case "cliente":
                    return "NomeCliente";

                case "loja":
                    return "NomeLoja";

                case "vendedor":
                    return "NomeFunc";

                case "tipovenda":
                    return "DescrTipoVenda";

                case "datafinalizacao":
                    return "DataFin";

                case "dataconfirmacao":
                    return "DataConf";

                case "idprojeto":
                case "idorcamento":
                case "total":
                case "datapedido":
                case "dataentrega":
                case "datapronto":
                case "dataliberacao":
                case "situacao":
                case "situacaoproducao":
                case "tipopedido":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
