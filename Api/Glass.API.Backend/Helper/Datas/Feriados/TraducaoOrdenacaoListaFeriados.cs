// <copyright file="TraducaoOrdenacaoListaFeriados.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Datas.Feriados
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de feriados.
    /// </summary>
    internal class TraducaoOrdenacaoListaFeriados : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaFeriados"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaFeriados(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Mes ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "descricao":
                    return "Descricao";

                case "mes":
                    return "Mes";

                case "ano":
                    return "Ano";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
