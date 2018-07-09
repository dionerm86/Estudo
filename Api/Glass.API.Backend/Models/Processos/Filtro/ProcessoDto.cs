// <copyright file="ProcessoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados básicos de processo para os controles de filtro.
    /// </summary>
    [DataContract(Name = "Processo")]
    public class ProcessoDto : IdCodigoDto
    {
        /// <summary>
        /// Obtém ou define o identificador da aplicação vinculada ao processo.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacao")]
        public int? IdAplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o código interno da aplicação vinculada ao processo.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoAplicacao")]
        public string CodigoAplicacao { get; set; }
    }
}
