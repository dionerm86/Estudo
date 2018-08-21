// <copyright file="DadosProjetoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.AmbientesPedido.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de projeto para um ambiente.
    /// </summary>
    [DataContract(Name = "DadosProjeto")]
    public class DadosProjetoDto
    {
        /// <summary>
        /// Obtém ou define o identificador do item de projeto vinculado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("idItemProjeto")]
        public int? IdItemProjeto { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
