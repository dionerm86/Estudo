// <copyright file="CadastroAtualizacaoFiscalDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do estoque fiscal de um produto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoFiscal")]
    public class CadastroAtualizacaoFiscalDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoFiscalDto>
    {
        /// <summary>
        /// Obtém ou define a quantidade em estoque fiscal do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeEstoqueFiscal")]
        public double QuantidadeEstoqueFiscal
        {
            get { return this.ObterValor(c => c.QuantidadeEstoqueFiscal); }
            set { this.AdicionarValor(c => c.QuantidadeEstoqueFiscal, value); }
        }

        /// <summary>
        /// Obtém ou define a quantidade do produto em posse de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePosseTerceiros")]
        public double QuantidadePosseTerceiros
        {
            get { return this.ObterValor(c => c.QuantidadePosseTerceiros); }
            set { this.AdicionarValor(c => c.QuantidadePosseTerceiros, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do participante em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idParticipante")]
        public int? IdParticipante
        {
            get { return this.ObterValor(c => c.IdParticipante); }
            set { this.AdicionarValor(c => c.IdParticipante, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de participante em posse de estoque do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoParticipante")]
        public Data.EFD.DataSourcesEFD.TipoPartEnum? TipoParticipante
        {
            get { return this.ObterValor(c => c.TipoParticipante); }
            set { this.AdicionarValor(c => c.TipoParticipante, value); }
        }
    }
}
