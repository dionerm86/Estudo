// <copyright file="DiaAnteriorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula dados de um dia anterior ao atual para fechamento do caixa.
    /// </summary>
    [DataContract(Name = "DiaAnterior")]
    public class DiaAnteriorDto : DadosDoDiaDto
    {
        /// <summary>
        /// Obtém ou define a data que o caixa ficou aberto.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCaixaAberto")]
        public DateTime? DataCaixaAberto { get; set; }
    }
}
