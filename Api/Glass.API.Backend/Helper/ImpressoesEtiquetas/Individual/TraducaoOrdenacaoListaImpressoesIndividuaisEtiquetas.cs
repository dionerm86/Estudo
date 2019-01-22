// <copyright file="TraducaoOrdenacaoListaImpressoesIndividuaisEtiquetas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ImpressoesEtiquetas.Individual
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de impressões individuais de etiquetas.
    /// </summary>
    internal class TraducaoOrdenacaoListaImpressoesIndividuaisEtiquetas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaImpressoesIndividuaisEtiquetas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaImpressoesIndividuaisEtiquetas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdPedido DESC, NumeroNfe DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "pedido":
                    return "IdPedido";

                case "notafiscal":
                    return "NumeroNFe";

                case "produto":
                    return "DescrProduto";

                case "altura":
                    return "Altura";

                case "largura":
                    return "Largura";

                case "processo":
                    return "CodProcesso";

                case "aplicacao":
                    return "CodAplicacao";

                case "quantidade":
                    return "Qtde";

                case "quantidadeimpresso":
                    return "QtdImpresso";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}