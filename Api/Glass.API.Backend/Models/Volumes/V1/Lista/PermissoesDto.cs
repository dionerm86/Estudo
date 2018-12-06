// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Volumes.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da tela de volumes.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se será permitido editar o volume.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }
    }
}
