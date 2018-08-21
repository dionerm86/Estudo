// <copyright file="TraducaoOrdenacaoListaObservacoesFinanceiro.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Pedidos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de observações financeiras.
    /// </summary>
    internal class TraducaoOrdenacaoListaObservacoesFinanceiro : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaObservacoesFinanceiro"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaObservacoesFinanceiro(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdObsFinanc DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "funcionario":
                    return "NomeFuncCad";

                case "data":
                    return "DataCad";

                case "motivo":
                case "observacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
