// <copyright file="TraducaoOrdenacaoListaOrdensCarga.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Carregamentos.OrdensCarga
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de ordens de carga.
    /// </summary>
    internal class TraducaoOrdenacaoListaOrdensCarga : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaOrdensCarga"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaOrdensCarga(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "oc.idOrdemCarga DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}