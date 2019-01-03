// <copyright file="TraducaoOrdenacaoListaPagamentos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Pagamentos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de pagamentos.
    /// </summary>
    internal class TraducaoOrdenacaoListaPagamentos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPagamentos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPagamentos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdPagto DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "idpagto":
                    return "IdPagto";

                case "descrformapagto":
                    return "DescrFormaPagto";

                case "desconto":
                    return "Desconto";

                case "obs":
                    return "Obs";

                case "valorpago":
                    return "ValorPago";

                case "datapagto":
                    return "DataPagto";

                case "descrsituacao":
                    return "Situacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}