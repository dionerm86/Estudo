// <copyright file="ClienteDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Clientes.V1.Filtro;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de cliente.
    /// </summary>
    [DataContract(Name = "Cliente")]
    public class ClienteDto : ClienteBaseDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se é exigido pagamento antecipado do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("exigePagamentoAntecipado")]
        public bool ExigePagamentoAntecipado { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de sinal mínimo exigido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualSinalMinimo")]
        public decimal PercentualSinalMinimo { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("telefone")]
        public string Telefone { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("endereco")]
        public string Endereco { get; set; }

        /// <summary>
        /// Obtém ou define a observação do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}
