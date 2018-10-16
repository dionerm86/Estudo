// <copyright file="EstoqueDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados de entrada para o método de validação de estoque.
    /// </summary>
    [DataContract(Name = "Estoque")]
    public class EstoqueDto
    {
        /// <summary>
        /// Obtém ou define o estoque real.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeReal")]
        public double QuantidadeReal { get; set; }

        /// <summary>
        /// Obtém ou define o estoque atual.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeAtual")]
        public double QuantidadeAtual { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroBarrasAluminio")]
        public int? NumeroBarrasAluminio { get; set; }

        /// <summary>
        /// Obtém ou define a unidade do estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("unidade")]
        public string Unidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o popup para falta de estoque deve ser exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPopupFaltaEstoque")]
        public bool ExibirPopupFaltaEstoque { get; set; }
    }
}
