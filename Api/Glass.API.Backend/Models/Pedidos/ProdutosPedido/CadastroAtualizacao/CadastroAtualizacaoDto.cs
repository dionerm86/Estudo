// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados para cadastro ou atualização de produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto
    {
        /// <summary>
        /// Obtém ou define os dados básicos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de produtos do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public double? Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define os dados do desconto por quantidade do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoPorQuantidade")]
        public PercentualValorDto DescontoPorQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public DadoRealECalculadoDto Altura { get; set; }

        /// <summary>
        /// Obtém ou define a área, em m², do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("areaEmM2")]
        public DadoRealECalculadoDto AreaEmM2 { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal? ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define o processo do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("processo")]
        public IdDto Processo { get; set; }

        /// <summary>
        /// Obtém ou define a aplicação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public IdDto Aplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente para o produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o valor total do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal? Total { get; set; }

        /// <summary>
        /// Obtém ou define a observação do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define os beneficiamentos do produto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public BeneficiamentosDto Beneficiamentos { get; set; }
    }
}
