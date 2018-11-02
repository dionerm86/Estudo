// <copyright file="TraducaoOrdenacaoListaModelosProjeto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos.ModelosProjeto
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de modelos de projeto.
    /// </summary>
    internal class TraducaoOrdenacaoListaModelosProjeto : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaModelosProjeto"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaModelosProjeto(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "Codigo ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "nome":
                    return "Descricao";

                case "situacao":
                case "codigo":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
