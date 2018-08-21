// <copyright file="RotaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados de rota para o controle de clientes.
    /// </summary>
    [DataContract(Name = "Rota")]
    public class RotaDto
    {
        /// <summary>
        /// Obtém ou define a data de rota.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime? Data { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a rota é "EntregaBalcao".
        /// </summary>
        [DataMember]
        [JsonProperty("entregaBalcao")]
        public bool? EntregaBalcao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa realiza a entrega para a rota.
        /// </summary>
        [DataMember]
        [JsonProperty("entrega")]
        public bool Entrega { get; set; }
    }
}
