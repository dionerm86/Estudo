// <copyright file="VendedorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Fornecedores.Lista
{
    /// <summary>
    /// Classe que encapsula dados do vendedor associado ao fornecedor.
    /// </summary>
    [DataContract(Name = "Vendedor")]
    public class VendedorDto
    {
        /// <summary>
        /// Obtém ou define o nome do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o telefone celular do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("celular")]
        public string Celular { get; set; }
    }
}
