// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Retalhos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissões para a lista de retalhos de produção.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se pode ser impresso um relatório detalhado sobre o retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o retalho de produção pode ser cancelado.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser marcada perda para o retalho de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("marcarPerda")]
        public bool MarcarPerda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a etiqueta que está sendo utilizada será exibida na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirEtiquetasUsando")]
        public bool ExibirEtiquetasUsando { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o retalho de produção possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o retalho de produção possui log de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}