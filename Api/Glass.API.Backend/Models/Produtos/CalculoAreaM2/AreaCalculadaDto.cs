using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.CalculoAreaM2
{
    /// <summary>
    /// Classe com o resultado do cálculo de área em m².
    /// </summary>
    [DataContract(Name = "AreaCalculada")]
    public class AreaCalculadaDto
    {
        /// <summary>
        /// Obtém ou define a área em m².
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2")]
        public double AreaM2 { get; set; }

        /// <summary>
        /// Obtém ou define a área (para cálculo) em m².
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2Calculo")]
        public double AreaM2Calculo { get; set; }

        /// <summary>
        /// Obtém ou define a área (para cálculo, desconsiderando a chapa de vidro) em m².
        /// </summary>
        [DataMember]
        [JsonProperty("areaM2CalculoSemChapaDeVidro")]
        public double AreaM2CalculoSemChapaDeVidro { get; set; }
    }
}
