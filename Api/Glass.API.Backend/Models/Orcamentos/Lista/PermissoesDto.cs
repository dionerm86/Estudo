// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Orcamentos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do orçamento.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento pode ser editado.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento pode ser impresso.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento pode imprimir a memória de cálculo.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirMemoriaCalculo")]
        public bool ImprimirMemoriaCalculo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento permite imprimir o projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirProjeto")]
        public bool ImprimirProjeto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o o orçamento permite cadastro de sugestão
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarSugestao")]
        public bool CadastrarSugestao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento pode gerar pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarPedido")]
        public bool GerarPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento permite o envio de email
        /// </summary>
        [DataMember]
        [JsonProperty("enviarEmail")]
        public bool EnviarEmail { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o orçamento possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
