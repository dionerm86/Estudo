// <copyright file="TraducaoOrdenacaoListaContasPagar.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ContasPagar
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de contas a pagar.
    /// </summary>
    internal class TraducaoOrdenacaoListaContasPagar : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaContasPagar"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaContasPagar(string ordenacao)
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
                    return "concat(coalesce(IdCompra,0), coalesce(IdPagto,0), coalesce(NumeroNf,0), coalesce(Nf,0), coalesce(NumBoleto,0), coalesce(IdCustoFixo,0))";

                case "fornecedor/transportador/funcionario":
                    return "coalesce(NomeFornec, NomeTransportador)";

                case "referente":
                    return "concat(coalesce(DescrPlanoConta,''), coalesce(Obs,''), coalesce(ObsCompra,''), coalesce(IdPagtoRestante,''), coalesce(IdCustoFixo,''))";

                case "formapagamento":
                    return "FormaPagtoCompra";

                case "parcelas":
                    return "NumParc, NumParcMax";

                case "valor":
                    return "ValorVenc";

                case "vencimento":
                    return "DataVenc";

                case "datacadastro":
                    return "DataCad";

                case "boletochegou":
                    return "BoletoChegou";

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