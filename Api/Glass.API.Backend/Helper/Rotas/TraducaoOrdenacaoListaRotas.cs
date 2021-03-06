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

                case "nome":
                    return "descricao";

                case "codigo":
                    return "CodInterno";

                case "numerodiasminimosparadataentrega":
                    return "NumeroMinimoDiasEntrega";

                case "observacao":
                    return "obs";

                case "situacao":
                case "distancia":
                case "diassemana":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
