// <copyright file="ItemDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.Venda
{
    /// <summary>
    /// Classe que encapsula os dados de um produto de venda (orçamento/pedido/PCP).
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ItemDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do material de projeto que gerou o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("idMaterialProjeto")]
        public int? IdMaterialProjeto { get; set; }

        /// <summary>
        /// Obtém ou define os dados de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de produtos de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public double Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do ambiente para o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeAmbiente")]
        public int QuantidadeAmbiente { get; set; }

        /// <summary>
        /// Obtém ou define os dados de desconto por quantidade do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoPorQuantidade")]
        public DescontoQuantidadeDto DescontoPorQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define a largura pra o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define os dados de altura para o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public AlturaDto Altura { get; set; }

        /// <summary>
        /// Obtém ou define os dados de área para o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("areaEmM2")]
        public AreaDto AreaEmM2 { get; set; }

        /// <summary>
        /// Obtém ou define o valor unitário para o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("valorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos para o processo do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("processo")]
        public IdCodigoDto Processo { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos para a aplicação do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public IdCodigoDto Aplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o código de pedido do cliente para o produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o valor total do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define os beneficiamentos do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public BeneficiamentosDto Beneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão do produto de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualComissao")]
        public double PercentualComissao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto possui filhos (no caso de produto composição/laminado).
        /// </summary>
        [DataMember]
        [JsonProperty("possuiFilhos")]
        public bool PossuiFilhos { get; set; }

        /// <summary>
        /// Recupera um objeto IdCodigoDto de acordo com os valores informados.
        /// </summary>
        /// <param name="id">O identificador do item.</param>
        /// <param name="codigo">O código do item.</param>
        /// <returns>Um objeto IdCodigoDto, se possível.</returns>
        protected IdCodigoDto ObterIdCodigo(int? id, string codigo)
        {
            return !id.HasValue || string.IsNullOrWhiteSpace(codigo)
                ? null
                : new IdCodigoDto
                {
                    Id = id.Value,
                    Codigo = codigo,
                };
        }
    }
}
