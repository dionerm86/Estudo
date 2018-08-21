// <copyright file="DadosEntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.ProdutosPedido.Observacao
{
    /// <summary>
    /// Classe que encapsula os dados de entrada para o método de salvar observação de produto de pedido.
    /// </summary>
    [DataContract(Name = "DadosEntrada")]
    public class DadosEntradaDto
    {
        /// <summary>
        /// Obtém ou define a observação para o produto de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}
