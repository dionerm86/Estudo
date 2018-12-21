// <copyright file="TraducaoOrdenacaoListaCompras.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Compras
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de compras.
    /// </summary>
    internal class TraducaoOrdenacaoListaCompras : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCompras"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCompras(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdCompra DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "num":
                    return "IdCompra";

                case "cotacao":
                    return "IdCotacaoCompra";

                case "pedido":
                    return "IdsPedido";

                case "fornecedor":
                    return "NomeFornec";

                case "loja":
                    return "NomeLoja";

                case "funcionario":
                    return "DescrUsuCad";

                case "total":
                    return "Total";

                case "dataentradafabrica":
                    return "DataFabrica";

                case "pagto":
                    return "DescrTipoCompra";

                case "data":
                    return "DataCad";

                case "situacao":
                    return "DescrSituacao";

                case "contabil":
                    return "Contabil";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}