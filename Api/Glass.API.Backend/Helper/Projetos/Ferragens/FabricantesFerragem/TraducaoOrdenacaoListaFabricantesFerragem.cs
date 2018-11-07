// <copyright file="TraducaoOrdenacaoListaFabricantesFerragem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos.Ferragens.FabricantesFerragem
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de fabricantes de ferragem.
    /// </summary>
    internal class TraducaoOrdenacaoListaFabricantesFerragem : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaFabricantesFerragem"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaFabricantesFerragem(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Nome ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "nome":
                    return "Nome";

                case "site":
                    return "Sitio";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
