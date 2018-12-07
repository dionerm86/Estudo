// <copyright file="TraducaoOrdenacaoListaImpressoesEtiquetas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ImpressoesEtiquetas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de impressões de etiquetas.
    /// </summary>
    internal class TraducaoOrdenacaoListaImpressoesEtiquetas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaImpressoesEtiquetas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaImpressoesEtiquetas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdImpressao DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdImpressao";

                case "loja":
                    return "NomeLoja";

                case "funcionario":
                    return "NomeFunc";

                case "dataimpressao":
                    return "Data";

                case "situacao":
                    return "DescrSituacao";

                case "tipoimpressao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
