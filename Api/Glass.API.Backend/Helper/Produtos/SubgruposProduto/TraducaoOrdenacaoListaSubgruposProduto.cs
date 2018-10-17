// <copyright file="TraducaoOrdenacaoListaSubgruposProduto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.SubgruposProduto
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de subgrupos de produto.
    /// </summary>
    internal class TraducaoOrdenacaoListaSubgruposProduto : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaSubgruposProduto"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaSubgruposProduto(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Descricao ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdSubgrupoProd";

                case "nome":
                    return "Descricao";

                case "tipocalculopedido":
                    return "TipoCalculo";

                case "tipocalculonotafiscal":
                    return "TipoCalculoNf";

                case "vidrotemperado":
                    return "IsVidroTemperado";

                case "produtoparaestoque":
                    return "ProdutosEstoque";

                case "bloquearestoque":
                    return "BloquearEstoque";

                case "alterarestoque":
                    return "NaoAlterarEstoque";

                case "alterarestoquefiscal":
                    return "NaoAlterarEstoqueFiscal";

                case "exibirmensagemestoque":
                    return "ExibirMensagemEstoque";

                case "geravolume":
                    return "GeraVolume";

                case "liberarpendenteproducao":
                    return "LiberarPendenteProducao";

                case "permitiritemrevendanavenda":
                    return "PermitirItemRevendaNaVenda";

                case "minimodiasentrega":
                    return "NumeroDiasMinimoEntrega";

                case "diasemanaentrega":
                    return "DiaSemanaEntrega";

                case "tipo":
                    return "TipoSubgrupo";

                case "cliente":
                    return "IdCli";

                case "loja":
                    return "IdLoja";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
