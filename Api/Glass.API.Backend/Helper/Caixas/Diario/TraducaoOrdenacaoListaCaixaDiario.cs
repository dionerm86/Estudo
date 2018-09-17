// <copyright file="TraducaoOrdenacaoListaCaixaDiario.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Caixas.Diario
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de movimentações de caixa diário.
    /// </summary>
    internal class TraducaoOrdenacaoListaCaixaDiario : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCaixaDiario"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCaixaDiario(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdCaixaDiario ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
