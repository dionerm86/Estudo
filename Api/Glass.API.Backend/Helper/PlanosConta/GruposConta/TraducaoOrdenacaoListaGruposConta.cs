// <copyright file="TraducaoOrdenacaoListaGruposConta.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.PlanosConta.GruposConta
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de grupos de conta.
    /// </summary>
    internal class TraducaoOrdenacaoListaGruposConta : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaGruposConta"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaGruposConta(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "NumeroSequencia, gc.IdGrupo"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdGrupo";

                case "nome":
                    return "Descricao";

                case "categoriaconta":
                    return "Categoria";

                case "exibirpontoequilibrio":
                    return "PontoEquilibrio";

                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
