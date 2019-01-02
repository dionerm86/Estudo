// <copyright file="DataCadastroMovimentacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Classe que encapsula data de cadastro e movimentação de estoque.
    /// </summary>
    [DataContract(Name = "DataCadastroMovimentacao")]
    public class DataCadastroMovimentacaoDto
    {
        /// <summary>
        /// Obtém ou define a data de cadastro.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastro")]
        public DateTime? Cadastro { get; set; }

        /// <summary>
        /// Obtém ou define a data da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("movimentacao")]
        public DateTime? Movimentacao { get; set; }
    }
}
