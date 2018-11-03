// <copyright file="TraducaoOrdenacaoListaFerragens.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Projetos.Ferragens
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de ferragens.
    /// </summary>
    internal class TraducaoOrdenacaoListaFerragens : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaFerragens"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaFerragens(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdFerragem ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "nome":
                    return "Nome";

                case "fabricante":
                    return "NomeFabricante";

                case "situacao":
                    return "Situacao";

                case "dataalteracao":
                    return "DataAlteracao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
