// <copyright file="TraducaoOrdenacaoListaAcertos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Acertos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de acertos.
    /// </summary>
    internal class TraducaoOrdenacaoListaAcertos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaAcertos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaAcertos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdAcerto DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdAcerto";

                case "cliente":
                    return "NomeCliente";

                case "total":
                    return "TotalAcerto";

                case "datacadastro":
                    return "DataCad";

                case "observacao":
                    return "Obs";

                case "funcionario":
                    return "Funcionario";

                case "situacao":
                    return "Situacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
