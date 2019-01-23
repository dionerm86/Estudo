// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pagamentos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um pagamento para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Pagamento")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="pagamento">A model de pagamentos.</param>
        internal ListaDto(Pagto pagamento)
        {
            this.Id = (int)pagamento.IdPagto;
            this.FormaPagamento = pagamento.DescrFormaPagto;
            this.Desconto = pagamento.Desconto;
            this.ValorPagamento = pagamento.ValorPago;
            this.DataPagamento = pagamento.DataPagto;
            this.Situacao = pagamento.DescrSituacao;
            this.Observacao = pagamento.Obs;
            this.Permissoes = new PermissoesDto
            {
                Cancelar = pagamento.CancelarVisible,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Pagto, pagamento.IdPagto, null),
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.Pagto, pagamento.IdPagto),
            };
        }

        /// <summary>
        /// Obtém ou define a forma de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public string FormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o desconto do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public decimal? Desconto { get; set; }

        /// <summary>
        /// Obtém ou define o valor pago.
        /// </summary>
        [DataMember]
        [JsonProperty("valorPagamento")]
        public decimal? ValorPagamento { get; set; }

        /// <summary>
        /// Obtém ou define a data de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPagamento")]
        public DateTime? DataPagamento { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissoes concedidas ao pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
