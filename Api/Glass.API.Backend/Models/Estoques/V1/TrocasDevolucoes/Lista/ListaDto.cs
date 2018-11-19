// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.TrocasDevolucoes.Lista
{
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="trocaDevolucao"></param>
        internal ListaDto(Data.Model.TrocaDevolucao trocaDevolucao)
        {
            this.Id = (int)trocaDevolucao.IdPedido;
            this.Cliente = new IdNomeDto
            {
                Id = (int)trocaDevolucao.IdCliente,
                Nome = trocaDevolucao.NomeCliente,
            };

            this.Loja = new IdNomeDto
            {
                Id = (int)trocaDevolucao.IdLoja,
                Nome = LojaDAO.Instance.GetNome(null, (uint)trocaDevolucao.IdLoja),
            };

            this.Situacao = new IdNomeDto
            {
                Id = trocaDevolucao.Situacao,
                Nome = trocaDevolucao.Situacao.ToString(),
            };

            this.Setor = trocaDevolucao.Setor;
            this.Origem = trocaDevolucao.DescrOrigemTrocaDevolucao;
            this.CreditoGerado = trocaDevolucao.CreditoGerado;
            this.DataErro = trocaDevolucao.DataErro;
            this.DataTroca = trocaDevolucao.DataTroca;
            this.Tipo = trocaDevolucao.Tipo;

            this.IdPedido = (int?)trocaDevolucao.IdPedido;
            this.ValorExcedente = trocaDevolucao.ValorExcedente;
            this.Permissoes = new PermissoesDto
            {
                Editar = trocaDevolucao.EditEnabled,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.TrocaDev, trocaDevolucao.IdTrocaDevolucao, null),
            };
        }

        /// <summary>
        /// Obtém ou define o código do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("setor")]
        public string Setor { get; set; }

        /// <summary>
        /// Obtém ou define o código do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("origem")]
        public string Origem { get; set; }

        /// <summary>
        /// Obtém ou define dados do tipo do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public IdNomeDto Loja { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("creditoGerado")]
        public decimal CreditoGerado { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("valorExcedente")]
        public decimal ValorExcedente { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("dataErro")]
        public DateTime? DataErro { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("dataTroca")]
        public DateTime? DataTroca { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de mercadoria do CFOP.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public int Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões do item.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
