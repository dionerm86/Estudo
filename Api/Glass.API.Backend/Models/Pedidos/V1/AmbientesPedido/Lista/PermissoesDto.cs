// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.Lista
{
    /// <summary>
    /// Classe com as permissões por ambiente.
    /// </summary>
    [DataContract]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o log de alterações será exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
