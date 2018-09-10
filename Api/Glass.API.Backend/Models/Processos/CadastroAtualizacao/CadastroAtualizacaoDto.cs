// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um processo de etiqueta.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo
        {
            get { return this.ObterValor(c => c.Codigo); }
            set { this.AdicionarValor(c => c.Codigo, value); }
        }

        /// <summary>
        /// Obtém ou define a descrição do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this.ObterValor(c => c.Descricao); }
            set { this.AdicionarValor(c => c.Descricao, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da aplicação do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacao")]
        public int? IdAplicacao
        {
            get { return this.ObterValor(c => c.IdAplicacao); }
            set { this.AdicionarValor(c => c.IdAplicacao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o processo cria um destaque na etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("destacarNaEtiqueta")]
        public bool DestacarNaEtiqueta
        {
            get { return this.ObterValor(c => c.DestacarNaEtiqueta); }
            set { this.AdicionarValor(c => c.DestacarNaEtiqueta, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o processo gera uma forma inexistente.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarFormaInexistente")]
        public bool GerarFormaInexistente
        {
            get { return this.ObterValor(c => c.GerarFormaInexistente); }
            set { this.AdicionarValor(c => c.GerarFormaInexistente, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o processo gera um arquivo de corte para mesa.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDeMesa")]
        public bool GerarArquivoDeMesa
        {
            get { return this.ObterValor(c => c.GerarArquivoDeMesa); }
            set { this.AdicionarValor(c => c.GerarArquivoDeMesa, value); }
        }

        /// <summary>
        /// Obtém ou define o número de dias úteis para calcular data de entrega do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasUteisDataEntrega")]
        public int NumeroDiasUteisDataEntrega
        {
            get { return this.ObterValor(c => c.NumeroDiasUteisDataEntrega); }
            set { this.AdicionarValor(c => c.NumeroDiasUteisDataEntrega, value); }
        }

        /// <summary>
        /// Obtém ou define o tipo de processo do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoProcesso")]
        public Data.Model.EtiquetaTipoProcesso? TipoProcesso
        {
            get { return this.ObterValor(c => c.TipoProcesso); }
            set { this.AdicionarValor(c => c.TipoProcesso, value); }
        }

        /// <summary>
        /// Obtém ou define os tipos de pedido do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidos")]
        public IEnumerable<Data.Model.Pedido.TipoPedidoEnum> TiposPedidos
        {
            get { return this.ObterValor(c => c.TiposPedidos); }
            set { this.AdicionarValor(c => c.TiposPedidos, value); }
        }

        /// <summary>
        /// Obtém ou define a situação do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }
    }
}
