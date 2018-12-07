// <copyright file="TraducaoOrdenacaoListaPendenciasCarregamentos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Carregamentos.Itens.Pendencias
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de carregamentos pendentes.
    /// </summary>
    internal class TraducaoOrdenacaoListaPendenciasCarregamentos : BaseTraducaoOrdenacao
    {
        public TraducaoOrdenacaoListaPendenciasCarregamentos(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "pc.idCarregamento DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}
