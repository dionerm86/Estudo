// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Parcelas.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma parcela.
    /// </summary>
    [DataContract(Name = "Cadastro")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id
        {
            get { return this.ObterValor(c => c.Id); }
            set { this.AdicionarValor(c => c.Id, value); }
        }

        /// <summary>
        /// Obtém ou define a descrição da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this.ObterValor(c => c.Descricao); }
            set { this.AdicionarValor(c => c.Descricao, value); }
        }

        /// <summary>
        /// Obtém ou define a situacao da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a parcela é a vista.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaAVista")]
        public bool ParcelaAVista
        {
            get { return this.ObterValor(c => c.ParcelaAVista); }
            set { this.AdicionarValor(c => c.ParcelaAVista, value); }
        }

        /// <summary>
        /// Obtém ou define os dias da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("dias")]
        public IEnumerable<int> Dias
        {
            get { return this.ObterValor(c => c.Dias); }
            set { this.AdicionarValor(c => c.Dias, value); }
        }

        /// <summary>
        /// Obtém ou define o desconto da parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public decimal Desconto
        {
            get { return this.ObterValor(c => c.Desconto); }
            set { this.AdicionarValor(c => c.Desconto, value); }
        }

        /// <summary>
        /// Obtém ou define o numero de parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroParcelas")]
        public int NumeroParcelas
        {
            get { return this.ObterValor(c => c.NumeroParcelas); }
            set { this.AdicionarValor(c => c.NumeroParcelas, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a parcela é padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelaPadrao")]
        public bool ParcelaPadrao
        {
            get { return this.ObterValor(c => c.ParcelaPadrao); }
            set { this.AdicionarValor(c => c.ParcelaPadrao, value); }
        }
    }
}
