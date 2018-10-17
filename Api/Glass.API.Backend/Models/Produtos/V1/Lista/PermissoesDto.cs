// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do produto.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se será exibido o link de reserva.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkReserva")]
        public bool ExibirLinkReserva { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibido o link de liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkLiberacao")]
        public bool ExibirLinkLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o produto possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
