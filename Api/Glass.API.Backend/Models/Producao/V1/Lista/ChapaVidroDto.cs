// <copyright file="ChapaVidroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados da chapa de vidro usada no corte da peça.
    /// </summary>
    [DataContract(Name = "ChapaVidro")]
    public class ChapaVidroDto
    {
        /// <summary>
        /// Obtém ou define o número da etiqueta da chapa de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroEtiqueta")]
        public string NumeroEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal da chapa de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroNotaFiscal")]
        public string NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o lote da chapa de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("lote")]
        public string Lote { get; set; }
    }
}
