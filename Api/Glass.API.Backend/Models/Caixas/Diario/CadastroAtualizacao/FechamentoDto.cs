// <copyright file="FechamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Diario.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de fechamento do caixa.
    /// </summary>
    [DataContract(Name = "Fechamento")]
    public class FechamentoDto
    {
        /// <summary>
        /// Obtém ou define o valor a ser transferido para o caixa geral.
        /// </summary>
        [DataMember]
        [JsonProperty("valorATransferirCaixaGeral")]
        public decimal? ValorATransferirCaixaGeral { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será fechado um dia anterior ao atual.
        /// </summary>
        [DataMember]
        [JsonProperty("diaAnterior")]
        public bool DiaAnterior { get; set; }
    }
}
