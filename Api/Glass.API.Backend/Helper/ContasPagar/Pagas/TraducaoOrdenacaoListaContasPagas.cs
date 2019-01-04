// <copyright file="TraducaoOrdenacaoListaContasPagas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ContasPagar.Pagas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de contas pagas.
    /// </summary>
    internal class TraducaoOrdenacaoListaContasPagas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaContasPagas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaContasPagas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "DataVenc ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdContaPg";

                case "referencia":
                    return "concat(coalesce(c.IdCompra,0), coalesce(c.IdPagto,0), coalesce(nf.NumeroNfe,0), coalesce(cmp.Nf,0), coalesce(c.NumBoleto,0), coalesce(c.IdCustoFixo,0))";

                case "fornecedor/transportador/funcionario":
                    return "coalesce(NomeFornec, NomeTransportador)";

                case "referente":
                    return "concat(coalesce(g.descricao,''), coalesce(pl.descricao,''), coalesce(c.Obs,''), coalesce(cmp.Obs,''), coalesce(c.IdPagtoRestante,''), coalesce(c.IdCustoFixo,''))";

                case "formapagamento":
                    return "FormaPagto";

                case "parcelas":
                    return "NumParc, NumParcMax";

                case "valor":
                    return "ValorVenc";

                case "valorpago":
                    return "cast(concat(coalesce(c.ValorPago,0), coalesce(c.Juros,0), coalesce(c.Multa,0)) as signed)";

                case "vencimento":
                    return "DataVenc";

                case "pagamento":
                    return "DataPagto";

                case "obs":
                    return "Obs";

                case "tipo":
                    return "DescricaoContaContabil";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}