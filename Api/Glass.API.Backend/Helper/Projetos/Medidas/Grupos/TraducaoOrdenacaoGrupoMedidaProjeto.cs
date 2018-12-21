// <copyright file="TraducaoOrdenacaoGrupoMedidaProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos.Medidas.Grupos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de grupos de medidas de projetos.
    /// </summary>
    internal class TraducaoOrdenacaoGrupoMedidaProjeto : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoGrupoMedidaProjeto"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoGrupoMedidaProjeto(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "DESCRICAO ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo)
            {
                case "descricao":
                    return "DESCRICAO";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}