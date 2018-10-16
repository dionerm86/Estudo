// <copyright file="TotaisAcumuladosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula totais acumulados.
    /// </summary>
    [DataContract(Name = "TotaisAcumulados")]
    public class TotaisAcumuladosDto
    {
        /// <summary>
        /// Obtém ou define o valor recebido em dinheiro.
        /// </summary>
        [DataMember]
        [JsonProperty("dinheiro")]
        public decimal Dinheiro { get; set; }

        /// <summary>
        /// Obtém ou define o valor recebido em cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cheque")]
        public decimal Cheque { get; set; }
    }
}
