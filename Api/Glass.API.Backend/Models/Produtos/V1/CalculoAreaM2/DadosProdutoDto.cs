// <copyright file="DadosProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.CalculoAreaM2
{
    /// <summary>
    /// Classe que encapsula os dados do produto para o serviço de cálculo de área.
    /// </summary>
    [DataContract(Name = "DadosProduto")]
    public class DadosProdutoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idProduto")]
        public int IdProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public decimal Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto é redondo.
        /// </summary>
        [DataMember]
        [JsonProperty("redondo")]
        public bool Redondo { get; set; }

        /// <summary>
        /// Obtém ou define a espessura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public decimal Espessura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cálculo deve considerar múltiplo de 5.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularMultiploDe5")]
        public bool CalcularMultiploDe5 { get; set; }

        /// <summary>
        /// Obtém ou define o número de beneficiamentos para considerar área mínima.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroBeneficiamentosParaAreaMinima")]
        public int NumeroBeneficiamentosParaAreaMinima { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de validação do cálculo.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoValidacao")]
        public string TipoValidacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados adicionais para validação.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosAdicionaisValidacao")]
        public string DadosAdicionaisValidacao { get; set; }
    }
}
