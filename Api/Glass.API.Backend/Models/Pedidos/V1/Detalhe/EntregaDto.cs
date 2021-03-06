﻿// <copyright file="EntregaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de entrega do pedido.
    /// </summary>
    [DataContract(Name = "Entrega")]
    public class EntregaDto
    {
        /// <summary>
        /// Obtém ou define o tipo de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public IdNomeDto Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime? Data { get; set; }

        /// <summary>
        /// Obtém ou define o valor de entrega (frete) do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }
    }
}
