// <copyright file="TraducaoOrdenacaoListaTipos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.CoresVidro
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de tipos de cliente.
    /// </summary>
    internal class TraducaoOrdenacaoListaTipos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaTipos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaTipos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdTipoCliente ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdTipoCliente";

                case "descricao":
                    return "Descricao";

                case "cobrarareaminima":
                    return "CobrarAreaMinima";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
