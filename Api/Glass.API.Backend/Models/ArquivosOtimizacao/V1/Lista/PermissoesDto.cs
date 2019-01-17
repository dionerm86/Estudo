// <copyright file="PermissoesDto.cs" company="Sync Softwares">
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
    /// Classe que encapsula os dados de permissão para o download do arquivo da lista de arquivos de otimização.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o download será efetuado.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkDownload")]
        public bool ExibirLinkDownload { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o download no ECutter será efetuado.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkECutter")]
        public bool ExibirLinkECutter { get; set; }
    }
}
