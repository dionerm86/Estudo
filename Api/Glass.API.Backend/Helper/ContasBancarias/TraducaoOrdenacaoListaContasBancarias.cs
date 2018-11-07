// <copyright file="TraducaoOrdenacaoListaContasBancarias.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.ContasBancarias
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de contas bancárias.
    /// </summary>
    internal class TraducaoOrdenacaoListaContasBancarias : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaContasBancarias"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaContasBancarias(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Nome ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigobanco":
                    return "CodBanco";

                case "codigoconvenio":
                    return "CodConvenio";

                case "codigocliente":
                    return "CodCliente";

                case "posto":
                    return "Posto";

                case "nome":
                case "loja":
                case "agencia":
                case "conta":
                case "titular":
                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
