// <copyright file="AlturaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1.Venda
{
    /// <summary>
    /// Classe que encapsula os dados de altura do produto.
    /// </summary>
    public class AlturaDto
    {
        /// <summary>
        /// Obtém ou define a altura para cálculo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("paraCalculo")]
        public decimal ParaCalculo { get; set; }

        /// <summary>
        /// Obtém ou define a altura real do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("real")]
        public decimal Real { get; set; }

        /// <summary>
        /// Obtém ou define a altura para exibição na lista do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("paraExibirNaLista")]
        public string ParaExibirNaLista { get; set; }
    }
}
