// <copyright file="FalhaHistoricoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma falha do histórico.
    /// </summary>
    [DataContract(Name = "FalhaHistorico")]
    public class FalhaHistoricoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FalhaHistoricoDto"/>.
        /// </summary>
        /// <param name="falha">Falha cujo os dados serão encapsulados.</param>
        public FalhaHistoricoDto(Glass.Integracao.Historico.Falha falha)
        {
            this.Tipo = falha.Tipo;
            this.Mensagem = falha.Mensagem;
            this.PilhaChamada = falha.PilhaChamada;
            this.FalhaInterna = falha.FalhaInterna != null ? new FalhaHistoricoDto(falha.FalhaInterna) : null;
        }

        /// <summary>
        /// Obtém o tipo da falha.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; }

        /// <summary>
        /// Obtém a mensagem da falha.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; }

        /// <summary>
        /// Obtém a pilha de chamada da falha.
        /// </summary>
        [DataMember]
        [JsonProperty("pilhaChamada")]
        public string PilhaChamada { get; }

        /// <summary>
        /// Obtém a falha interna.
        /// </summary>
        [DataMember]
        [JsonProperty("falhaInterna")]
        public FalhaHistoricoDto FalhaInterna { get; }
    }
}
