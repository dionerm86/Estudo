// <copyright file="TraducaoOrdenacaoListaNotasFiscais.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.NotasFiscais
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de notas fiscais.
    /// </summary>
    internal class TraducaoOrdenacaoListaNotasFiscais : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaNotasFiscais"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaNotasFiscais(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdNf DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdNf";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
