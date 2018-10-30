// <copyright file="DadosBeneficiamentosDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Beneficiamentos.V1.Total
{
    /// <summary>
    /// Classe que encapsula os dados de beneficiamentos para o cálculo de total.
    /// </summary>
    [DataContract(Name = "DadosBeneficiamentos")]
    public class DadosBeneficiamentosDto
    {
        /// <summary>
        /// Obtém ou define o beneficiamento que foi aplicado.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamento")]
        public V1.Filtro.BeneficiamentoDto Beneficiamento { get; set; }

        /// <summary>
        /// Obtém ou define o beneficiamento selecionado no controle.
        /// </summary>
        [DataMember]
        [JsonProperty("itensSelecionados")]
        public IEnumerable<Genericas.V1.ItemBeneficiamentoDto> ItensSelecionados { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o beneficiamento será cobrado.
        /// </summary>
        [DataMember]
        [JsonProperty("cobrar")]
        public bool CobrarBeneficiamento { get; set; }
    }
}
