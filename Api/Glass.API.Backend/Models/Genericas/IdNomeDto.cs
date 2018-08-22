// <copyright file="IdNomeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe que encapsula dados básicos (ID e nome) de um item.
    /// </summary>
    [DataContract(Name = "IdNome")]
    public class IdNomeDto : IdDto
    {
        /// <summary>
        /// Obtém ou define o nome do item.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
