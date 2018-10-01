// <copyright file="TraducaoOrdenacaoListaProducao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Producao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de pedidos.
    /// </summary>
    internal class TraducaoOrdenacaoListaProducao : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaProducao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaProducao(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get
            {
                return "ppp.IdProdPedProducao DESC";
            }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "":
                    return "";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
