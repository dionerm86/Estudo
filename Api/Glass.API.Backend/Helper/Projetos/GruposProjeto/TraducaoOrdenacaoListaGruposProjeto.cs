// <copyright file="TraducaoOrdenacaoListaGruposProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos.GruposProjeto
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de grupos de projeto.
    /// </summary>
    internal class TraducaoOrdenacaoListaGruposProjeto : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaGruposProjeto"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaGruposProjeto(string ordenacao)
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
                case "nome":
                    return "Descricao";

                case "situacao":
                case "boxpadrao":
                case "esquadria":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
