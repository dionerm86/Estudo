// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Lista
{
    /// <summary>
    /// Classe que encapsula os dados do produto associado a uma chapa de matéria prima.
    /// </summary>
    [DataContract(Name = "ProdutoDto")]
    public class ProdutoDto
    {
        /// <summary>
        /// Obtém ou define o código do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
