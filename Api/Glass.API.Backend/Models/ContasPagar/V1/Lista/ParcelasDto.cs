// <copyright file="ParcelasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das parcelas para a lista de contas a pagar.
    /// </summary>
    [DataContract(Name = "Parcelas")]
    public class ParcelasDto
    {
        /// <summary>
        /// Obtém ou define número de parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("numero")]
        public int? Numero { get; set; }

        /// <summary>
        /// Obtém ou define o total de parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public int? Total { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se as informações da parcela serão exibidas.
        /// </summary>
        [DataMember]
        [JsonProperty("exibir")]
        public bool? Exibir { get; set; }
    }
}