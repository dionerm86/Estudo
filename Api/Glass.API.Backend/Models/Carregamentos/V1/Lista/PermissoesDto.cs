// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da lista de carregamentos.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o carregamento pode ser faturado.
        /// </summary>
        [DataMember]
        [JsonProperty("faturarCarregamento")]
        public bool FaturarCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o faturamento do carregamento pode ser impresso.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirFaturamento")]
        public bool ImprimirFaturamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o as pendências do carregamento podem ser visualizadas.
        /// </summary>
        [DataMember]
        [JsonProperty("visualizarPendenciasCarregamento")]
        public bool VisualizarPendenciasCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o carregamento possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o carregamento possui log de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}
