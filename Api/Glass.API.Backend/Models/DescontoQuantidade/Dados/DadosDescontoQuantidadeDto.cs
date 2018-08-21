// <copyright file="DadosDescontoQuantidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.DescontoQuantidade.Dados
{
    /// <summary>
    /// Classe que encapsula os dados para o controle de desconto por quantidade.
    /// </summary>
    [DataContract(Name = "DadosDescontoQuantidade")]
    public class DadosDescontoQuantidadeDto
    {
        /// <summary>
        /// Obtém ou define o percentual máximo de desconto por quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualDescontoPorQuantidade")]
        public double PercentualDescontoPorQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de desconto de tabela do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualDescontoTabela")]
        public double PercentualDescontoTabela { get; set; }
    }
}
