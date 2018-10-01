// <copyright file="TraducaoOrdenacaoListaCoresFerragem.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.CoresFerragem
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de cores de ferragem.
    /// </summary>
    internal class TraducaoOrdenacaoListaCoresFerragem : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCoresFerragem"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCoresFerragem(string ordenacao)
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
                case "id":
                    return "IdCorFerragem";

                case "sigla":
                    return "Sigla";

                case "descricao":
                    return "Descricao";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
