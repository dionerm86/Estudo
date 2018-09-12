// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.PedidosConferencia;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.PedidosConferencia.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de pedidos em conferência.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPedidosConferencia(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido em conferência.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente do pedido em conferência.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente do pedido em conferência.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do pedido em conferência.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor do pedido em conferência.
        /// </summary>
        [JsonProperty("idVendedor")]
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do conferente do pedido em conferência.
        /// </summary>
        [JsonProperty("idConferente")]
        public int? IdConferente { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido em conferência.
        /// </summary>
        [JsonProperty("situacao")]
        public PedidoEspelho.SituacaoPedido? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido comercial.
        /// </summary>
        [JsonProperty("situacaoPedidoComercial")]
        public PedidoEspelho.SituacaoPedido[] SituacaoPedidoComercial { get; set; }

        /// <summary>
        /// Obtém ou define os ids de processo dos produtos do pedido em conferência.
        /// </summary>
        [JsonProperty("idsProcesso")]
        public int[] IdsProcesso { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do filtro de entrega do pedido.
        /// </summary>
        [JsonProperty("periodoEntregaInicio")]
        public DateTime? PeriodoEntregaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do filtro de entrega do pedido.
        /// </summary>
        [JsonProperty("periodoEntregaFim")]
        public DateTime? PeriodoEntregaFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do filtro de entrega de fábrica do pedido.
        /// </summary>
        [JsonProperty("periodoFabricaInicio")]
        public DateTime? PeriodoFabricaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do filtro de entrega de fábrica do pedido.
        /// </summary>
        [JsonProperty("periodoFabricaFim")]
        public DateTime? PeriodoFabricaFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do filtro de finalização da conferência do pedido.
        /// </summary>
        [JsonProperty("periodoFinalizacaoConferenciaInicio")]
        public DateTime? PeriodoFinalizacaoConferenciaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do filtro de finalização da conferência do pedido.
        /// </summary>
        [JsonProperty("periodoFinalizacaoConferenciaFim")]
        public DateTime? PeriodoFinalizacaoConferenciaFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do filtro de cadastro da conferência do pedido.
        /// </summary>
        [JsonProperty("periodoCadastroConferenciaInicio")]
        public DateTime? PeriodoCadastroConferenciaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do filtro de cadastro da conferência do pedido.
        /// </summary>
        [JsonProperty("periodoCadastroConferenciaFim")]
        public DateTime? PeriodoCadastroConferenciaFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do filtro de cadastro do pedido do comercial.
        /// </summary>
        [JsonProperty("periodoCadastroPedidoInicio")]
        public DateTime? PeriodoCadastroPedidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do filtro de cadastro do pedido do comercial.
        /// </summary>
        [JsonProperty("periodoCadastroPedidoFim")]
        public DateTime? PeriodoCadastroPedidoFim { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverão ser buscados apenas pedidos em conferência sem anexo.
        /// </summary>
        [JsonProperty("pedidosSemAnexo")]
        public bool PedidosSemAnexo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverão ser buscados apenas pedidos com produtos a serem comprados.
        /// </summary>
        [JsonProperty("pedidosAComprar")]
        public bool PedidosAComprar { get; set; }

        /// <summary>
        /// Obtém ou define a situação CNC do pedido comercial.
        /// </summary>
        [JsonProperty("situacaoCnc")]
        public PedidoEspelho.SituacaoCncEnum[] SituacaoCnc { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de alteração da situação do projeto do CNC do pedido em conferência.
        /// </summary>
        [JsonProperty("periodoProjetoCncInicio")]
        public DateTime? PeriodoProjetoCncInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de alteração da situação do projeto do CNC do pedido em conferência.
        /// </summary>
        [JsonProperty("periodoProjetoCncFim")]
        public DateTime? PeriodoProjetoCncFim { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do pedido em conferência.
        /// </summary>
        [JsonProperty("tiposPedido")]
        public Data.Model.Pedido.TipoPedidoEnum[] TiposPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da rota do pedido em conferência.
        /// </summary>
        [JsonProperty("idsRota")]
        public int[] IdsRota { get; set; }

        /// <summary>
        /// Obtém ou define a origem do pedido em conferência.
        /// </summary>
        [JsonProperty("origemPedido")]
        public int? OrigemPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para buscar pedidos conferidos.
        /// </summary>
        [JsonProperty("pedidosConferidos")]
        public int PedidosConferidos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido em conferência importado foi conferido.
        /// </summary>
        [JsonProperty("pedidoImportacaoConferido")]
        public bool PedidoImportacaoConferido { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda do pedido em conferência.
        /// </summary>
        [JsonProperty("tipoVenda")]
        public Data.Model.Pedido.TipoVendaPedido? TipoVenda { get; set; }
    }
}
