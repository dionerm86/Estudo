// <copyright file="TraducaoOrdenacaoListaPosicoesMateriaPrima.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.MateriaPrima.Posicao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de posição de matéria prima.
    /// </summary>
    internal class TraducaoOrdenacaoListaPosicoesMateriaPrima : BaseTraducaoOrdenacao
	{
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPosicoesMateriaPrima"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPosicoesMateriaPrima(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "DescrCorVidro ASC"; }
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