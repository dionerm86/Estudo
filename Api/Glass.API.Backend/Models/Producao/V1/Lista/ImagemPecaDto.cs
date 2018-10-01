// <copyright file="ImagemPecaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de imagem da peça.
    /// </summary>
    [DataContract(Name = "ImagemPeca")]
    public class ImagemPecaDto
    {
        /// <summary>
        /// Obtém ou define a URL da imagem da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça possui arquivo SVG associado.
        /// </summary>
        [DataMember]
        [JsonProperty("possuiSvg")]
        public bool PossuiSvg { get; set; }
    }
}
