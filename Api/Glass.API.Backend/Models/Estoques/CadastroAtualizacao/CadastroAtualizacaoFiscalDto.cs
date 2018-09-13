// <copyright file="CadastroAtualizacaoFiscalDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do estoque fiscal de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoFiscal")]
    public class CadastroAtualizacaoFiscalDto
    {
        /// <summary>
        /// Obtém ou define a quantidade em estoque fiscal do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoqueFiscal")]
        public decimal QuantidadeEstoqueFiscal { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade do produto em posse de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePosseTerceiros")]
        public decimal QuantidadePosseTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do participante em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idParticipante")]
        public int? IdParticipante { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de participante em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoParticipante")]
        public Data.EFD.DataSourcesEFD.TipoPartEnum? TipoParticipante { get; set; }
    }
}
