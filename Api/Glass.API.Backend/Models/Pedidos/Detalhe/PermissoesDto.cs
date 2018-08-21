// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Detalhe
{
    /// <summary>
    /// Classe que encapsula as permissões do pedido detalhado.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ser colocado em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("colocarEmConferencia")]
        public bool ColocarEmConferencia { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente pode ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarCliente")]
        public bool AlterarCliente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o tipo de venda pode ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarTipoVenda")]
        public bool AlterarTipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o vendedor pode ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarVendedor")]
        public bool AlterarVendedor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o desconto pode ser alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarDesconto")]
        public bool AlterarDesconto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ser editado (usado para validação).
        /// </summary>
        [DataMember]
        [JsonProperty("podeEditar")]
        public bool PodeEditar { get; set; }
    }
}
