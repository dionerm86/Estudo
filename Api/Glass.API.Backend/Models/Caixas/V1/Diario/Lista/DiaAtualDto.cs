// <copyright file="DiaAtualDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula dados do dia atual para fechamento do caixa.
    /// </summary>
    [DataContract(Name = "DiaAtual")]
    public class DiaAtualDto : DadosDoDiaDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se existem movimentações no caixa no dia atual.
        /// </summary>
        [DataMember]
        [JsonProperty("existemMovimentacoes")]
        public bool ExistemMovimentacoes { get; set; }
    }
}
