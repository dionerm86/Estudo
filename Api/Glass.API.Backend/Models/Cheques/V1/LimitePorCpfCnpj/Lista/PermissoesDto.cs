// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das permissões para a tela de listagem de limite de cheques por cpf/cnpj.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o log de alterações poderá ser acessado.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}