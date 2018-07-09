// <copyright file="ObraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Obras.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados de obra para seleção.
    /// </summary>
    [DataContract(Name = "Obra")]
    public class ObraDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ObraDto"/>.
        /// </summary>
        /// <param name="obra">A model de obras.</param>
        public ObraDto(Obra obra)
        {
            this.Id = (int)obra.IdObra;
            this.Descricao = obra.Descricao;
            this.Saldo = obra.Saldo;
            this.TotalPedidosAbertosObra = obra.TotalPedidosAbertos;
        }

        /// <summary>
        /// Obtém ou define o identificador da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o saldo da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define o total de pedidos em aberto da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("totalPedidosAbertosObra")]
        public decimal TotalPedidosAbertosObra { get; set; }
    }
}
