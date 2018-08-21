// <copyright file="LarguraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.Filtro
{
    /// <summary>
    /// Classe que encapsula as informações sobre a largura do produto.
    /// </summary>
    [DataContract(Name = "Largura")]
    public class LarguraDto : QuantidadeDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o campo pode ser editado.
        /// </summary>
        [DataMember]
        [JsonProperty("podeEditar")]
        public bool PodeEditar { get; set; }

        /// <summary>
        /// Obtém ou define o valor padrão do campo, se houver.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public double? Valor { get; set; }
    }
}
