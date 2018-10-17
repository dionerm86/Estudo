// <copyright file="TotalAreaM2Dto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.ContagemPecas
{
    /// <summary>
    /// Classe que encapsula os dados de área, em m², para a contagem de peças.
    /// </summary>
    [DataContract(Name = "AreaM2")]
    public class TotalAreaM2Dto
    {
        /// <summary>
        /// Obtém ou define o total de área real das peças.
        /// </summary>
        [DataMember]
        [JsonProperty("real")]
        public decimal Real { get; set; }

        /// <summary>
        /// Obtém ou define o total de área para cálculo das peças.
        /// </summary>
        [DataMember]
        [JsonProperty("paraCalculo")]
        public decimal ParaCalculo { get; set; }
    }
}
