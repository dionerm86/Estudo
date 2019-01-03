// <copyright file="DatasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das datas para a lista de contas a pagar.
    /// </summary>
    [DataContract(Name = "Parcelas")]
    public class DatasDto
    {
        /// <summary>
        /// Obtém ou define a data de vencimento da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("vencimento")]
        public DateTime? Vencimento { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastro")]
        public DateTime? Cadastro { get; set; }
    }
}