// <copyright file="AlturaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Filtro
{
    /// <summary>
    /// Classe que encapsula as informações sobre a altura do produto.
    /// </summary>
    [DataContract(Name = "Altura")]
    public class AlturaDto : LarguraDto
    {
        /// <summary>
        /// Obtém ou define o fator de arredondamento, se houver.
        /// </summary>
        [DataMember]
        [JsonProperty("fatorArredondamento")]
        public decimal? FatorArredondamento { get; set; }
    }
}
