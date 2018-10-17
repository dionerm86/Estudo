// <copyright file="DadosImagemDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Imagens.V1.Exibicao
{
    /// <summary>
    /// Classe que contém os dados de uma imagem para o controle de exibição.
    /// </summary>
    [DataContract(Name = "DadosImagem")]
    public class DadosImagemDto
    {
        /// <summary>
        /// Obtém ou define a URL da imagem.
        /// </summary>
        [DataMember]
        [JsonProperty("urlImagem")]
        public string UrlImagem { get; set; }

        /// <summary>
        /// Obtém ou define a legenda da imagem.
        /// </summary>
        [DataMember]
        [JsonProperty("legenda")]
        public string Legenda { get; set; }
    }
}
