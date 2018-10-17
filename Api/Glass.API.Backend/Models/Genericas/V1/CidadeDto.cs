// <copyright file="CidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula os dados de cidades para o endereço.
    /// </summary>
    [DataContract(Name = "Cidade")]
    public class CidadeDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define a UF da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("uf")]
        public string Uf { get; set; }
    }
}
