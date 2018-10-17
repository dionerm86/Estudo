// <copyright file="DadosContagemDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.ContagemPecas
{
    /// <summary>
    /// Classe que encapsula os dados da contagem de um tipo de peça.
    /// </summary>
    [DataContract(Name = "Dados")]
    public class DadosContagemDto
    {
        /// <summary>
        /// Obtém ou define o número de peças.
        /// </summary>
        [DataMember]
        [JsonProperty("numero")]
        public long Numero { get; set; }

        /// <summary>
        /// Obtém ou define os dados de área das peças.
        /// </summary>
        [DataMember]
        [JsonProperty("areaEmM2")]
        public TotalAreaM2Dto AreaEmM2 { get; set; }
    }
}
