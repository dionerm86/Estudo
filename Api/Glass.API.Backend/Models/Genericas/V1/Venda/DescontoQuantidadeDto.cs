// <copyright file="DescontoQuantidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1.Venda
{
    /// <summary>
    /// Classe que encapsula os dados de desconto por quantidade do produto do pedido.
    /// </summary>
    [DataContract(Name = "DescontoQuantidade")]
    public class DescontoQuantidadeDto
    {
        /// <summary>
        /// Obtém ou define o percentual de desconto por quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("percentual")]
        public decimal Percentual { get; set; }

        /// <summary>
        /// Obtém ou define o valor de desconto por quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }
}
