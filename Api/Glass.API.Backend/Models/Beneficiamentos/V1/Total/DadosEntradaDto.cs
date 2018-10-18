// <copyright file="DadosEntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Beneficiamentos.V1.Total
{
    /// <summary>
    /// Classe que encapsula os dados de entrada para o cálculo de total dos beneficiamentos.
    /// </summary>
    [DataContract(Name = "DadosEntrada")]
    public class DadosEntradaDto
    {
        /// <summary>
        /// Obtém ou define os dados adicionais para o cálculo do beneficiamento.
        /// </summary>
        [DataMember]
        [JsonProperty("dadosCalculo")]
        public DadosCalculoDto DadosCalculo { get; set; }

        /// <summary>
        /// Obtém ou define o beneficiamento que foi aplicado.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public IEnumerable<DadosBeneficiamentosDto> Beneficiamentos { get; set; }
    }
}
