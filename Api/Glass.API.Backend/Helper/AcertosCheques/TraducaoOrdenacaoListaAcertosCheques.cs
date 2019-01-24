// <copyright file="TraducaoOrdenacaoListaAcertosCheques.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.AcertosCheques
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de acertos de cheques.
    /// </summary>
    internal class TraducaoOrdenacaoListaAcertosCheques : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaAcertosCheques"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaAcertosCheques(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "idAcertoCheque desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "num":
                    return "IdAcertoCheque";

                case "funcionario":
                    return "NomeFunc";

                case "data":
                    return "DataAcerto";

                case "valor":
                    return "ValorAcerto";

                case "observacao":
                    return "Obs";

                case "situacao":
                    return "Situacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}