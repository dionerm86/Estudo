// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Tipos.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de tipos de cliente.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="tipoCliente">O tipo de cliente que será retornado.</param>
        public ListaDto(TipoCliente tipoCliente)
        {
            this.Id = tipoCliente.IdTipoCliente;
            this.Descricao = tipoCliente.Descricao;
            this.CobrarAreaMinima = tipoCliente.CobrarAreaMinima;
        }

        /// <summary>
        /// Obtém ou define a descrição do tipo de cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se esse tipo de cliente cobra área mínima.
        /// </summary>
        [DataMember]
        [JsonProperty("cobrarAreaMinima")]
        public bool CobrarAreaMinima { get; set; }
    }
}
