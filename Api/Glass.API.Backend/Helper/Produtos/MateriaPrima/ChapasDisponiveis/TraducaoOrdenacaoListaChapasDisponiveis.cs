// <copyright file="TraducaoOrdenacaoListaChapasDisponiveis.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.MateriaPrima.ChapasDisponiveis
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de CFOP's.
    /// </summary>
    internal class TraducaoOrdenacaoListaChapasDisponiveis : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaChapasDisponiveis"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaChapasDisponiveis(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "desc"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            return this.OrdenacaoPadrao;
        }
    }
}
