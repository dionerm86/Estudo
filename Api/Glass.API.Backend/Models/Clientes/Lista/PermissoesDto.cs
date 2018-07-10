// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do cliente.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o cliente pode ser ativado/inativado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarSituacao")]
        public bool AlterarSituacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá cadastrar/alterar desconto de tabela para o cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarDescontoTabela")]
        public bool CadastrarDescontoTabela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
