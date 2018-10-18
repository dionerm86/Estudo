// <copyright file="TraducaoOrdenacaoListaProcessos.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Processos
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de processos de etiquetas.
    /// </summary>
    internal class TraducaoOrdenacaoListaProcessos : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaProcessos"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaProcessos(string ordenacao)
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

                case "aplicacao":
                    return "CodInternoAplicacao";

                case "destacarnaetiqueta":
                    return "DestacarEtiqueta";

                case "descricao":
                case "gerarformainexistente":
                case "gerararquivodemesa":
                case "numerodiasuteisdataentrega":
                case "tipoprocesso":
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
