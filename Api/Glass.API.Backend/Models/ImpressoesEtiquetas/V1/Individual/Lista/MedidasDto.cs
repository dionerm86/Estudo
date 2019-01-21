// <copyright file="MedidasDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Individual.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes as medidas para a tela de listagem de impressão individual de etiquetas.
    /// </summary>
    [DataContract(Name = "Medidas")]
    public class MedidasDto
    {
        /// <summary>
        /// Obtém ou define a altura do produto associado a etiqueta para impressão.
        /// </summary>
        [JsonProperty("altura")]
        public decimal Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto associado a etiqueta para impressão.
        /// </summary>
        [JsonProperty("largura")]
        public decimal Largura { get; set; }
    }
}