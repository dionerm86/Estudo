// <copyright file="ProcessoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.V1.Filtro
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
        [JsonProperty("aplicacao")]
        public IdCodigoDto Aplicacao { get; set; }
    }
}
