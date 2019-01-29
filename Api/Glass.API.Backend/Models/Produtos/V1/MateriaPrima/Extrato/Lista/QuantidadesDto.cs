// <copyright file="QuantidadesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das quantidades para um item da lista de extrato de movimentações de chapa.
    /// </summary>
    [DataContract(Name = "Quantidades")]
    public class QuantidadesDto
    {
        /// <summary>
        /// Obtém ou define a a quantidade utilizada referente as chapas associadas a uma determinada espessura e cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("utilizada")]
        public string Utilizada { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade disponível referente as chapas associadas a uma determinada espessura e cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("disponivel")]
        public string Disponivel { get; set; }
    }
}