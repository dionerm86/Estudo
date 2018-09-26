// <copyright file="TraducaoOrdenacaoListaCoresAluminio.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.CoresAluminio
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de cores de alumínio.
    /// </summary>
    internal class TraducaoOrdenacaoListaCoresAluminio : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaCoresAluminio"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaCoresAluminio(string ordenacao)
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
                    return "IdCorAluminio";

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
