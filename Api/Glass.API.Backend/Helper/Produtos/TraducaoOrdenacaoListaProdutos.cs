// <copyright file="TraducaoOrdenacaoListaProdutos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de produtos.
    /// </summary>
    internal class TraducaoOrdenacaoListaProdutos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaProdutos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaProdutos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdProd DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigo":
                    return "CodInterno";

                case "descricaogrupo":
                    return "TipoProduto";

                case "custofornecedor":
                    return "Custofabase";

                case "custocomimpostos":
                    return "CustoCompra";

                case "quantidadereserva":
                    return "Reserva";

                case "quantidadeliberacao":
                    return "Liberacao";

                case "estoque":
                    return "QtdeEstoque";

                case "estoquedisponivel":
                case "descricao":
                case "altura":
                case "largura":
                case "valoratacado":
                case "valorbalcao":
                case "valorobra":
                case "valorreposicao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
