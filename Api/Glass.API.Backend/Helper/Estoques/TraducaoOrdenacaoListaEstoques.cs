// <copyright file="TraducaoOrdenacaoListaEstoques.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Estoques
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de estoques.
    /// </summary>
    internal class TraducaoOrdenacaoListaEstoques : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaEstoques"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaEstoques(string ordenacao)
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
                case "codigointernoproduto":
                    return "CodInternoProd";

                case "descricaoproduto":
                    return "DescrProduto";

                case "descricaogrupoproduto":
                    return "DescrGrupoProd";

                case "estoqueminimo":
                    return "EstMinimo";

                case "estoquem2":
                    return "M2";

                case "quantidadereserva":
                    return "Reserva";

                case "quantidadeliberacao":
                    return "Liberacao";

                case "quantidadeestoque":
                    return "QtdEstoque";

                case "quantidadeestoquefiscal":
                    return "EstoqueFiscal";

                case "quantidadedefeito":
                    return "Defeito";

                case "quantidadeposseterceiros":
                    return "QtdePosseTerceiros";

                case "idloja":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
