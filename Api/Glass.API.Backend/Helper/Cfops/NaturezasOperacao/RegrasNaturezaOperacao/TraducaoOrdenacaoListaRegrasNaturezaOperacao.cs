// <copyright file="TraducaoOrdenacaoListaRegrasNaturezaOperacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Cfops.NaturezasOperacao.RegrasNaturezaOperacao
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de regras de natureza de operação.
    /// </summary>
    internal class TraducaoOrdenacaoListaRegrasNaturezaOperacao : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaRegrasNaturezaOperacao"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaRegrasNaturezaOperacao(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdLoja, IdTipoCliente, IdGrupoProd, IdSubgrupoProd"; }
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
