// <copyright file="TraducaoOrdenacaoListaContasRecebidas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ContasReceber
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de contas recebidas.
    /// </summary>
    internal class TraducaoOrdenacaoListaContasRecebidas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaContasRecebidas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaContasRecebidas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "DataVec DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "idcomissao":
                case "numeronfe":
                    return campo;

                case "numeroparcela":
                    return "NumParc";

                case "nomecliente":
                    return "NomeCli";

                case "formapagamento":
                    return "FormaPagto";

                case "valorvencimento":
                    return "ValorVec";

                case "datavencimento":
                    return "DataVec";

                case "valorrecebido":
                    return "ValorRec";

                case "datarecebimento":
                    return "DataRec";

                case "recebidapor":
                    return "NomeFunc";

                case "localizacao":
                    return "DestinoRec";

                case "numeroarquivoremessa":
                    return "NumArquivoRemessaCnab";

                case "observacao":
                    return "obs";

                case "descricaocomissao":
                    return "PercentualComissao";

                case "tipocontabil":
                    return "DescricaoContaContabil";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
