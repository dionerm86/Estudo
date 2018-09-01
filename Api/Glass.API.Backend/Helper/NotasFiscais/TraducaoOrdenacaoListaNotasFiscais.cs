// <copyright file="TraducaoOrdenacaoListaNotasFiscais.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.NotasFiscais
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de notas fiscais.
    /// </summary>
    internal class TraducaoOrdenacaoListaNotasFiscais : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaNotasFiscais"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaNotasFiscais(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "DataEmissao DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "id":
                    return "IdNf";

                case "codigocfop":
                    return "codcfop";

                case "usuariocadastro":
                    return "descrusucad";

                case "nomedestinatario":
                    return "nomedestrem";

                case "dataentradasaida":
                    return "datasaidaent";

                case "basedecalculoicms":
                    return "bcicms";

                case "basedecalculoicmsst":
                    return "bcicmsst";

                case "numeronfe":
                case "serie":
                case "modelo":
                case "tipodocumento":
                case "nomeemitente":
                case "dataemissao":
                case "valoricms":
                case "valoricmsst":
                case "valoripi":
                case "totalnota":
                case "situacao":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
