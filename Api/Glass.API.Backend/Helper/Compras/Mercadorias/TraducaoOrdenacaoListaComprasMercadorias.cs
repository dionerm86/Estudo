// <copyright file="TraducaoOrdenacaoListaComprasMercadorias.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Compras.Mercadorias
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de compras de mercadorias.
    /// </summary>
    internal class TraducaoOrdenacaoListaComprasMercadorias : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaComprasMercadorias"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaComprasMercadorias(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "c.IdCompra desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdCompra";

                case "pedido":
                    return "IdPedidoEspelho";

                case "fornecedor":
                    return "NomeFornec";

                case "loja":
                    return "NomeLoja";

                case "funcionario":
                    return "DescrUsuCad";

                case "total":
                    return "Total";

                case "pagamento":
                    return "DescrTipoCompra";

                case "data":
                    return "DataCad";

                case "situacao":
                    return "DescrSituacao";

                case "contabil":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
