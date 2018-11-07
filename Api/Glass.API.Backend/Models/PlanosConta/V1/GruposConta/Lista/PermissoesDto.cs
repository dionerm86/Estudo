// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PlanosConta.V1.GruposConta.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da lista de grupos de conta.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de conta poderá ser excluído.
        /// </summary>
        [DataMember]
        [JsonProperty("excluir")]
        public bool Excluir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá editar apenas a opção de exibir grupo de conta no ponto de equilíbrio.
        /// </summary>
        [DataMember]
        [JsonProperty("editarApenasExibirPontoEquilibrio")]
        public bool EditarApenasExibirPontoEquilibrio { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de conta possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
