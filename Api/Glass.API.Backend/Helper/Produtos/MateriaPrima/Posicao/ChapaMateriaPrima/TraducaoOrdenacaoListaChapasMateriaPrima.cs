// <copyright file="TraducaoOrdenacaoListaChapasMateriaPrima.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.MateriaPrima.Posicao.ChapaMateriaPrima
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de chapas de matéria prima.
    /// </summary>
    internal class TraducaoOrdenacaoListaChapasMateriaPrima : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaChapasMateriaPrima"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaChapasMateriaPrima(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return string.Empty; }
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