// <copyright file="ObservacoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das observações para a lista de contas pagas.
    /// </summary>
    [DataContract(Name = "Observacoes")]
    public class ObservacoesDto
    {
        /// <summary>
        /// Obtém ou define a descrição/observação da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("contaPaga")]
        public string ContaPaga { get; set; }

        /// <summary>
        /// Obtém ou define a descrição/observação do desconto.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public string Desconto { get; set; }

        /// <summary>
        /// Obtém ou define descrição/observação do acréscimo.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public string Acrescimo { get; set; }
    }
}