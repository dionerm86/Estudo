// <copyright file="TraducaoOrdenacaoListaPlanosConta.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.PlanosConta
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de planos de conta.
    /// </summary>
    internal class TraducaoOrdenacaoListaPlanosConta : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPlanosConta"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPlanosConta(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Descricao ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigo":
                    return "IdContaGrupo";

                case "grupoconta":
                    return "IdGrupo";

                case "nome":
                    return "Descricao";

                case "exibirdre":
                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
