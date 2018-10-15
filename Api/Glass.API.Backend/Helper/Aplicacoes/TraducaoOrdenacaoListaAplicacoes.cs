// <copyright file="TraducaoOrdenacaoListaAplicacoes.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Aplicacoes
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de processos de etiquetas.
    /// </summary>
    internal class TraducaoOrdenacaoListaAplicacoes : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaAplicacoes"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaAplicacoes(string ordenacao)
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
                    return "CodInterno";

                case "destacarnaetiqueta":
                    return "DestacarEtiqueta";

                case "descricao":
                case "gerarformainexistente":
                case "naopermitirfastdelivery":
                case "numerodiasuteisdataentrega":
                case "situacao":
                    return campo;

                case "tipospedidos":
                    return "TipoPedido";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
