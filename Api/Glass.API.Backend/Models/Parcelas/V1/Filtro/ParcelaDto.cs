// <copyright file="ParcelaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Parcelas.V1.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados básicos da parcela para os controles de filtro.
    /// </summary>
    [DataContract(Name = "Parcela")]
    public class ParcelaDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define o número de parcelas que esta parcela tem.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroParcelas")]
        public int NumeroParcelas { get; set; }

        /// <summary>
        /// Obtém ou define o número de dias de uma parcela para outra.
        /// </summary>
        [DataMember]
        [JsonProperty("dias")]
        public IEnumerable<int> Dias { get; set; }
    }
}
