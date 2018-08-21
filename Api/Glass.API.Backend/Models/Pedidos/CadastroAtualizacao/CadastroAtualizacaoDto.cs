// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Pedidos.Detalhe;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização do pedido.
    /// </summary>
    [DataContract(Name = "Cadastro")]
    public class CadastroAtualizacaoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do cliente do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da obra do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idObra")]
        public int? IdObra { get; set; }

        /// <summary>
        /// Obtém ou define a data do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPedido")]
        public DateTime? DataPedido { get; set; }

        /// <summary>
        /// Obtém ou define se o pedido é "fast delivery".
        /// </summary>
        [DataMember]
        [JsonProperty("fastDelivery")]
        public bool? FastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define se o pedido possui "deve transferir".
        /// </summary>
        [DataMember]
        [JsonProperty("deveTransferir")]
        public bool? DeveTransferir { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public Data.Model.Pedido.TipoPedidoEnum? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVenda")]
        public Data.Model.Pedido.TipoVendaPedido? TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("idVendedor")]
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do medidor.
        /// </summary>
        [DataMember]
        [JsonProperty("idMedidor")]
        public int? IdMedidor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário comprador.
        /// </summary>
        [DataMember]
        [JsonProperty("idFuncionarioComprador")]
        public int? IdFuncionarioComprador { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do transportador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idTransportador")]
        public int? IdTransportador { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido original de revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedidoRevenda")]
        public int? IdPedidoRevenda { get; set; }

        /// <summary>
        /// Obtém ou define se o pedido deve gerar um pedido de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarPedidoCorte")]
        public bool? GerarPedidoCorte { get; set; }

        /// <summary>
        /// Obtém ou define os dados de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("entrega")]
        public EntregaDto Entrega { get; set; }

        /// <summary>
        /// Obtém ou define os dados da forma de pagamento do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public FormaPagamentoDto FormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define os dados de desconto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public DescontoAcrescimoDto Desconto { get; set; }

        /// <summary>
        /// Obtém ou define os dados de acréscimo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public DescontoAcrescimoDto Acrescimo { get; set; }

        /// <summary>
        /// Obtém ou define os dados de comissionado do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("comissionado")]
        public ComissionadoDto Comissionado { get; set; }

        /// <summary>
        /// Obtém ou define a observação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacaoLiberacao")]
        public string ObservacaoLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoObra")]
        public EnderecoDto EnderecoObra { get; set; }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("sinal")]
        public SinalDto Sinal { get; set; }
    }
}
