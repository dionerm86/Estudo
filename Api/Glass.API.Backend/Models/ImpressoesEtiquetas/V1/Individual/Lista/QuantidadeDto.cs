// <copyright file="QuantidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Individual.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes a quantidade para a tela de listagem de impressão individual de etiquetas.
    /// </summary>
    [DataContract(Name = "Quantidade")]
    public class QuantidadeDto
	{
        /// <summary>
        /// Obtém ou define a quantidade total do produto associado a etiqueta para impressão.
        /// </summary>
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade impressa do produto associado a etiqueta para impressão.
        /// </summary>
        [JsonProperty("impressa")]
        public int Impressa { get; set; }
    }
}