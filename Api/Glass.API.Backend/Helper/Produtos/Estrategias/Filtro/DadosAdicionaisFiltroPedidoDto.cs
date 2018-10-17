// <copyright file="DadosAdicionaisFiltroPedidoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Clientes.V1.Filtro;
using Newtonsoft.Json;
using static Glass.Data.Model.Pedido;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.Filtro
{
    /// <summary>
    /// Classe com os dados adicionais para a validação de pedido do filtro de produtos.
    /// </summary>
    internal class DadosAdicionaisFiltroPedidoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do pedido.
        /// </summary>
        [JsonProperty("idLoja")]
        public int IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da obra do pedido.
        /// </summary>
        [JsonProperty("idObra")]
        public int? IdObra { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pedido.
        /// </summary>
        [JsonProperty("tipoPedido")]
        public TipoPedidoEnum TipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda do pedido.
        /// </summary>
        [JsonProperty("tipoVenda")]
        public TipoVendaPedido TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto está sendo inserido em um ambiente.
        /// </summary>
        [JsonProperty("ambiente")]
        public bool ProdutoAmbiente { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de entrega do pedido.
        /// </summary>
        [JsonProperty("tipoEntrega")]
        public TipoEntregaPedido TipoEntrega { get; set; }

        /// <summary>
        /// Obtém ou define os dados de cliente do pedido.
        /// </summary>
        [JsonProperty("cliente")]
        public ClienteBaseDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de desconto por quantidade do produto.
        /// </summary>
        [JsonProperty("percentualDescontoPorQuantidade")]
        public decimal PercentualDescontoPorQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define a área, em m², que será desconsiderada no cálculo do tamanho máximo da obra.
        /// </summary>
        [JsonProperty("areaM2DesconsiderarObra")]
        public decimal? AreaEmM2DesconsiderarObra { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [JsonProperty("altura")]
        public decimal? Altura { get; set; }
    }
}
