// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da lista de medidas de projeto.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a medida de projeto pode ser alterada.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a medida de projeto possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
