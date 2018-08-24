// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Liberacoes.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma liberação para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Liberacao")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="liberacao">A model de liberação.</param>
        internal ListaDto(LiberarPedido liberacao)
        {
            this.Id = (int)liberacao.IdLiberarPedido;
            this.Cliente = new IdNomeDto
            {
                Id = (int)liberacao.IdCliente,
                Nome = liberacao.NomeCliente,
            };

            this.Funcionario = new IdNomeDto
            {
                Id = null,
                Nome = liberacao.NomeFunc,
            };

            this.DescricaoPagamento = liberacao.DescrPagto;
            this.Situacao = liberacao.DescrSituacao;
            this.DataLiberacao = liberacao.DataLiberacao;
            this.ValorIcms = liberacao.ValorIcms;
            this.Total = liberacao.Total;
            this.NotasGeradas = liberacao.NotasFiscaisGeradas;

            this.Permissoes = new PermissoesDto
            {
                Imprimir = liberacao.ImprimirRelatorioLiberacaoVisible,
                ExibirNotaPromissoria = liberacao.ExibirNotaPromissoria,
                Cancelar = liberacao.CancelarVisible,
                ExibirBoleto = liberacao.BoletoVisivel,
                ExibirReenvioEmail = liberacao.ExibirReenvioEmail,
                ExibirNfeGerada = LiberarPedidoDAO.Instance.TemNfe(liberacao.IdLiberarPedido) && liberacao.Situacao != (int)LiberarPedido.SituacaoLiberarPedido.Cancelado && PedidoConfig.LiberarPedido,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.LiberacaoReenvioEmail, liberacao.IdLiberarPedido, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o cliente da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário que efetuou a liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public IdNomeDto Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do pagamento da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoPagamento")]
        public string DescricaoPagamento { get; set; }

        /// <summary>
        /// Obtém ou define a situação da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a data da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("dataLiberacao")]
        public DateTime DataLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define o valor do ICMS da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("valorIcms")]
        public decimal ValorIcms { get; set; }

        /// <summary>
        /// Obtém ou define o total da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define as notas geradas a partir da liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("notasGeradas")]
        public string NotasGeradas { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
