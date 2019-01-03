// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Lista
{
    /// <summary>
    /// Classe que encapsula as permissões para a tela de listagem de contas a pagar.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a conta a pagar pode ser editada.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a data de vencimento da conta a pagar pode ser editada.
        /// </summary>
        [DataMember]
        [JsonProperty("editarDataVencimento")]
        public bool EditarDataVencimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a conta a pagar possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}