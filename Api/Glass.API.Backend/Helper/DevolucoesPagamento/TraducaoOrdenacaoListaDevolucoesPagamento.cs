// <copyright file="TraducaoOrdenacaoListaDevolucoesPagamento.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.DevolucoesPagamento
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de devoluções de pagamento.
    /// </summary>
    internal class TraducaoOrdenacaoListaDevolucoesPagamento : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaDevolucoesPagamento"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaDevolucoesPagamento(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "dp.idDevolucaoPagto Desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdDevolucaoPagto";

                case "cliente":
                    return "NomeCliente";

                case "data":
                    return "DataCad";

                case "valor":
                    return "Valor";

                case "situacao":
                    return campo;

                case "funcionario":
                    return "NomeFunc";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}