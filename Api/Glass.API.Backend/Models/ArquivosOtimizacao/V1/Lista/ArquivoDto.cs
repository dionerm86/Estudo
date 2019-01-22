// <copyright file="ArquivoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ArquivosOtimizacao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados de arquivos para a lista de arquivos de otimização.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ArquivoDto
    {
        /// <summary>
        /// Obtém ou define a extensão do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("extensao")]
        public string Extensao { get; set; }

        /// <summary>
        /// Obtém ou define o caminho do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("caminho")]
        public string Caminho { get; set; }
    }
}
