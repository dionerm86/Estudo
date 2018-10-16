// <copyright file="FechamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de fechamento do caixa.
    /// </summary>
    [DataContract(Name = "Fechamento")]
    public class FechamentoDto
    {
        /// <summary>
        /// Obtém ou define dados do dia atual do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("diaAtual")]
        public DiaAtualDto DiaAtual { get; set; }

        /// <summary>
        /// Obtém ou define dados de um dia anterior ao atual (caso não tenha sido fechado) do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("diaAnterior")]
        public DiaAnteriorDto DiaAnterior { get; set; }
    }
}
