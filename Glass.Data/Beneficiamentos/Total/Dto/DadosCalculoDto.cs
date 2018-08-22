// <copyright file="DadosCalculoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.Data.Beneficiamentos.Total.Dto
{
    /// <summary>
    /// Classe que encapsula os dados de produto para o cálculo de total.
    /// </summary>
    [DataContract(Name = "DadosCalculo")]
    public class DadosCalculoDto
    {
        /// <summary>
        /// Obtém ou define os dados do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public DadosProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente é de revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("clienteRevenda")]
        public bool ClienteRevenda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o valor do beneficiamento
        /// está disponível para edição no controle.
        /// </summary>
        [DataMember]
        [JsonProperty("valorBeneficiamentoEstaEditavelNoControle")]
        public bool ValorBeneficiamentoEstaEditavelNoControle { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de desconto ou acréscimo do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualDescontoAcrescimoCliente")]
        public double PercentualDescontoAcrescimoCliente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o percentual de desconto ou acréscimo do cliente
        /// é usado para o cálculo do valor dos beneficiamentos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDescontoAcrescimoClienteNosBeneficiamentos")]
        public bool UsarDescontoAcrescimoClienteNosBeneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de entrega do pedido/orçamento/PCP para o cálculo do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoEntrega")]
        public Model.Pedido.TipoEntregaPedido TipoEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão aplicado no pedido/orçamento/PCP.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualComissao")]
        public double PercentualComissao { get; set; }
    }
}
