// <copyright file="DadosExecucaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.ExecucaoOperacaoIntegracao
{
    /// <summary>
    /// Representa os dados para a execução da operação.
    /// </summary>
    [DataContract(Name = "DadosExecucao")]
    public class DadosExecucaoDto
    {
        /// <summary>
        /// Obtém ou define a operação que será executada.
        /// </summary>
        [DataMember]
        [JsonProperty("operacao")]
        public string Operacao { get; set; }

        /// <summary>
        /// Obtém ou define os parâmetros da execução.
        /// </summary>
        [DataMember]
        [JsonProperty("parametros")]
        public IEnumerable<string> Parametros { get; set; }
    }
}
