// <copyright file="DadoRealECalculadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de altura do produto.
    /// </summary>
    [DataContract(Name = "Altura")]
    public class DadoRealECalculadoDto
    {
        /// <summary>
        /// Obtém ou define a altura real do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("real")]
        public double? Real { get; set; }

        /// <summary>
        /// Obtém ou define a altura para cálculo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("paraCalculo")]
        public double? ParaCalculo { get; set; }
    }
}
