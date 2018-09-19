// <copyright file="TraducaoOrdenacaoListaSinais.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Sinais
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de sinais.
    /// </summary>
    internal class TraducaoOrdenacaoListaSinais : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaSinais"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaSinais(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdSinal DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdSinal";

                case "cliente":
                    return "NomeCliente";

                case "total":
                    return "TotalSinal";

                case "datacadastro":
                    return "DataCad";

                case "observacao":
                    return "Obs";

                case "situacao":
                    return "Situacao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
