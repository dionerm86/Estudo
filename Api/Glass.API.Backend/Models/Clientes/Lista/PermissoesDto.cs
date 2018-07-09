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
        /// Obtém ou define um valor que indica se o cliente pode ser excluído.
        /// </summary>
        [DataMember]
        [JsonProperty("excluir")]
        public bool Excluir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente pode ser inativado.
        /// </summary>
        [DataMember]
        [JsonProperty("inativar")]
        public bool Inativar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente pode ser impresso.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá cadastrar/alterar desconto de tabela para o cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarDescontoTabela")]
        public bool CadastrarDescontoTabela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido anexar imagens ao cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("anexarImagens")]
        public bool AnexarImagens { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cadastrar sugestões para o cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarSugestoes")]
        public bool CadastrarSugestoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido consultar o preço de tabela do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarPrecoTabela")]
        public bool ConsultarPrecoTabela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido visualizar o total comprado pelo cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirTotalComprado")]
        public bool ExibirTotalComprado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
