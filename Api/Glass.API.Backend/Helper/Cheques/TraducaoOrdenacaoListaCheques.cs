﻿// <copyright file="TraducaoOrdenacaoListaCheques.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Cheques
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de cheques.
    /// </summary>
    internal class TraducaoOrdenacaoListaCheques : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCheques"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCheques(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdCheque DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "loja":
                    return "idLoja";

                case "cliente":
                    return "idCliente";

                case "fornecedor":
                    return "idFornecedor";

                case "numero":
                    return "num";

                case "datavencimento":
                    return "dataVenc";

                case "observacao":
                    return "obs";

                case "situacao":
                    return "situacao";

                case "agencia":
                case "conta":
                case "titular":
                case "banco":
                case "cpfcnpj":
                case "valor":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
