// <copyright file="DirecaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ArquivosOtimizacao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula dados de direção para a lista de arquivos de otimização.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class DirecaoDto : IdDto
    {
        /// <summary>
        /// Obtém ou define a descrição da direção do arquivo de otimização.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
