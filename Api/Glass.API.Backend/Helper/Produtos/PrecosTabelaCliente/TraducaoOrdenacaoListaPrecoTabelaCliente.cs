// <copyright file="TraducaoOrdenacaoListaPrecoTabelaCliente.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Produtos.PrecosTabelaCliente
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de preço de tabela por cliente.
    /// </summary>
    internal class TraducaoOrdenacaoListaPrecoTabelaCliente : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaPrecoTabelaCliente"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        public TraducaoOrdenacaoListaPrecoTabelaCliente(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "CodInterno ASC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "codigo":
                    return "CodInterno";

                case "descricao":
                    return "Descricao";

                case "grupo":
                    return "DescrGrupo";

                case "tipovalortabela":
                    return "TituloValorTabelaUtilizado";

                case "valororiginal":
                    return "ValorOriginalUtilizado";

                case "valortabela":
                    return "ValorTabelaUtilizado";

                case "altura":
                    return "Altura";

                case "largura":
                    return "Largura";

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}