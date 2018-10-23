// <copyright file="RestricoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula restrições do setor.
    /// </summary>
    [DataContract(Name = "Restricoes")]
    public class RestricoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se é obrigatório fazer leitura neste setor antes de ler a peça em qualquer setor posterior.
        /// </summary>
        [DataMember]
        [JsonProperty("impedirAvanco")]
        public bool ImpedirAvanco { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é obrigatório informar rota neste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("informarRota")]
        public bool InformarRota { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é obrigatório informar o cavalete neste setor.
        /// </summary>
        [DataMember]
        [JsonProperty("informarCavalete")]
        public bool InformarCavalete { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é permitido fazer leitura neste setor mesmo não estando no roteiro da peça.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirLeituraForaRoteiro")]
        public bool PermitirLeituraForaRoteiro { get; set; }
    }
}
