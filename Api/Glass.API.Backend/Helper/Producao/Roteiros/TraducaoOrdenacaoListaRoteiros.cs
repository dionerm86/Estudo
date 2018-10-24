// <copyright file="TraducaoOrdenacaoListaRoteiros.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao.Roteiros
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de roteiros.
    /// </summary>
    internal class TraducaoOrdenacaoListaRoteiros : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaRoteiros"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaRoteiros(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "CodProcesso ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "processo":
                    return "CodProcesso";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
