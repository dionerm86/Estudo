// <copyright file="CidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados da Cidade.
    /// </summary>
    [DataContract(Name = "Cidade")]
    public class CidadeDto
    {
        /// <summary>
        /// Obtém ou define a unidade federativa da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("uf")]
        public string Uf { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
