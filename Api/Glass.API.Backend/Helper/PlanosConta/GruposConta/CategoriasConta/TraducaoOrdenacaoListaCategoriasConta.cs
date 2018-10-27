// <copyright file="TraducaoOrdenacaoListaCategoriasConta.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.PlanosConta.GruposConta.CategoriasConta
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de categorias de conta.
    /// </summary>
    internal class TraducaoOrdenacaoListaCategoriasConta : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCategoriasConta"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCategoriasConta(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "NumeroSequencia, gc.IdCategoria"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdCategoria";

                case "nome":
                    return "Descricao";

                case "tipo":
                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
