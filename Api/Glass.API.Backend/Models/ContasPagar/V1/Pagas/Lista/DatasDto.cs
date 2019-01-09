// <copyright file="DatasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das datas para a lista de contas pagas.
    /// </summary>
    [DataContract(Name = "Datas")]
    public class DatasDto
    {
        /// <summary>
        /// Obtém ou define a data de vencimento da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("vencimento")]
        public DateTime? Vencimento { get; set; }

        /// <summary>
        /// Obtém ou define a data de pagamento da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("pagamento")]
        public DateTime? Pagamento { get; set; }
    }
}