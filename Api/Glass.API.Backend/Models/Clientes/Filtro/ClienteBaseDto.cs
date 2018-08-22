// <copyright file="ClienteBaseDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados básicos de cliente.
    /// </summary>
    [DataContract(Name = "ClienteBase")]
    public class ClienteBaseDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o cliente é revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("revenda")]
        public bool Revenda { get; set; }
    }
}
