// <copyright file="TraducaoOrdenacaoListaRotas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Rotas
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de rotas.
    /// </summary>
    internal class TraducaoOrdenacaoListaRotas : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaRotas"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaRotas(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "CodInterno ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdRota";

                case "codigo":
                    return "CodInterno";

                case "minimodiasentrega":
                    return "NumeroMinimoDiasEntrega";

                case "observacao":
                    return "obs";

                case "descricao":
                case "situacao":
                case "distancia":
                case "diasSemana":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
