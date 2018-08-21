// <copyright file="TraducaoOrdenacaoListaAmbientesPedido.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Helper.Pedidos.AmbientesPedido
{
    /// <summary>
    /// Classe que realiza a tradução dos campos de ordenação para a lista de ambientes do pedido.
    /// </summary>
    internal class TraducaoOrdenacaoListaAmbientesPedido : BaseTraducaoOrdenacao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="TraducaoOrdenacaoListaAmbientesPedido"/>.
        /// </summary>
        /// <param name="ordenacao">A ordenação realizada na tela.</param>
        internal TraducaoOrdenacaoListaAmbientesPedido(string ordenacao)
            : base(ordenacao)
        {
        }

        /// <inheritdoc/>
        protected override string OrdenacaoPadrao
        {
            get { return "IdPedido DESC"; }
        }

        /// <inheritdoc/>
        protected override string TraduzirCampo(string campo)
        {
            switch (campo.ToLowerInvariant())
            {
                case "valorprodutos":
                    return "totalProdutos";

                case "quantidade":
                    return "qtde";

                case "processo":
                    return "idProcesso";

                case "aplicacao":
                    return "idAplicacao";

                case "ambiente":
                case "descricao":
                case "largura":
                case "altura":
                case "redondo":
                case "acrescimo":
                case "desconto":
                    return campo;

                default:
                    return this.OrdenacaoPadrao;
            }
        }
    }
}
