// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Glass.API.Backend.Models.Pedidos.V1.Detalhe;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização do pedido.
    /// </summary>
    [DataContract(Name = "Cadastro")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do cliente do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int? IdCliente
        {
            get { return this.ObterValor(c => c.IdCliente); }
            set { this.AdicionarValor(c => c.IdCliente, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da loja do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int? IdLoja
        {
            get { return this.ObterValor(c => c.IdLoja); }
            set { this.AdicionarValor(c => c.IdLoja, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da obra do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idObra")]
        public int? IdObra
        {
            get { return this.ObterValor(c => c.IdObra); }
            set { this.AdicionarValor(c => c.IdObra, value); }
        }

        /// <summary>
        /// Obtém ou define a data do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPedido")]
        public DateTime DataPedido
        {
            get { return this.ObterValor(c => c.DataPedido); }
            set { this.AdicionarValor(c => c.DataPedido, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido é "fast delivery".
        /// </summary>
        [DataMember]
        [JsonProperty("fastDelivery")]
        public bool FastDelivery
        {
            get { return this.ObterValor(c => c.FastDelivery); }
            set { this.AdicionarValor(c => c.FastDelivery, value); }
        }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente
        {
            get { return this.ObterValor(c => c.CodigoPedidoCliente); }
            set { this.AdicionarValor(c => c.CodigoPedidoCliente, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui "deve transferir".
        /// </summary>
        [DataMember]
        [JsonProperty("deveTransferir")]
        public bool DeveTransferir
        {
            get { return this.ObterValor(c => c.DeveTransferir); }
            set { this.AdicionarValor(c => c.DeveTransferir, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public Data.Model.Pedido.TipoPedidoEnum Tipo
        {
            get { return this.ObterValor(c => c.Tipo); }
            set { this.AdicionarValor(c => c.Tipo, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de venda do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVenda")]
        public Data.Model.Pedido.TipoVendaPedido? TipoVenda
        {
            get { return this.ObterValor(c => c.TipoVenda); }
            set { this.AdicionarValor(c => c.TipoVenda, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("idVendedor")]
        public int IdVendedor
        {
            get { return this.ObterValor(c => c.IdVendedor); }
            set { this.AdicionarValor(c => c.IdVendedor, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do medidor.
        /// </summary>
        [DataMember]
        [JsonProperty("idMedidor")]
        public int? IdMedidor
        {
            get { return this.ObterValor(c => c.IdMedidor); }
            set { this.AdicionarValor(c => c.IdMedidor, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do funcionário comprador.
        /// </summary>
        [DataMember]
        [JsonProperty("idFuncionarioComprador")]
        public int? IdFuncionarioComprador
        {
            get { return this.ObterValor(c => c.IdFuncionarioComprador); }
            set { this.AdicionarValor(c => c.IdFuncionarioComprador, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do transportador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idTransportador")]
        public int? IdTransportador
        {
            get { return this.ObterValor(c => c.IdTransportador); }
            set { this.AdicionarValor(c => c.IdTransportador, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido original de revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedidoRevenda")]
        public int? IdPedidoRevenda
        {
            get { return this.ObterValor(c => c.IdPedidoRevenda); }
            set { this.AdicionarValor(c => c.IdPedidoRevenda, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido deve gerar um pedido de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarPedidoCorte")]
        public bool GerarPedidoCorte
        {
            get { return this.ObterValor(c => c.GerarPedidoCorte); }
            set { this.AdicionarValor(c => c.GerarPedidoCorte, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("entrega")]
        public EntregaDto Entrega
        {
            get { return this.ObterValor(c => c.Entrega); }
            set { this.AdicionarValor(c => c.Entrega, value); }
        }

        /// <summary>
        /// Obtém ou define os dados da forma de pagamento do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public FormaPagamentoDto FormaPagamento
        {
            get { return this.ObterValor(c => c.FormaPagamento); }
            set { this.AdicionarValor(c => c.FormaPagamento, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de desconto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public DescontoAcrescimoDto Desconto
        {
            get { return this.ObterValor(c => c.Desconto); }
            set { this.AdicionarValor(c => c.Desconto, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de acréscimo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public DescontoAcrescimoDto Acrescimo
        {
            get { return this.ObterValor(c => c.Acrescimo); }
            set { this.AdicionarValor(c => c.Acrescimo, value); }
        }

        /// <summary>
        /// Obtém ou define os dados de comissionado do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("comissionado")]
        public ComissionadoDto Comissionado
        {
            get { return this.ObterValor(c => c.Comissionado); }
            set { this.AdicionarValor(c => c.Comissionado, value); }
        }

        /// <summary>
        /// Obtém ou define a observação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao
        {
            get { return this.ObterValor(c => c.Observacao); }
            set { this.AdicionarValor(c => c.Observacao, value); }
        }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacaoLiberacao")]
        public string ObservacaoLiberacao
        {
            get { return this.ObterValor(c => c.ObservacaoLiberacao); }
            set { this.AdicionarValor(c => c.ObservacaoLiberacao, value); }
        }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoObra")]
        public EnderecoDto EnderecoObra
        {
            get { return this.ObterValor(c => c.EnderecoObra); }
            set { this.AdicionarValor(c => c.EnderecoObra, value); }
        }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("sinal")]
        public SinalDto Sinal
        {
            get { return this.ObterValor(c => c.Sinal); }
            set { this.AdicionarValor(c => c.Sinal, value); }
        }
    }
}
