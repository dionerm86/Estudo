// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.ContasPagar.V1;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das contas a pagar para a tela de listagem.
    /// </summary>
    [DataContract(Name = "ContasPagas")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="contaPaga">A model de contas pagas.</param>
        internal ListaDto(Data.Model.ContasPagar contaPaga)
        {
            this.Id = (int)contaPaga.IdContaPg;
            this.IdPlanoConta = (int?)contaPaga.IdConta;
            this.Referencia = contaPaga.Referencia;
            this.Fornecedor = new IdNomeDto
            {
                Id = (int?)contaPaga.IdFornec,
                Nome = contaPaga.NomeFornec,
            };

            this.Transportador = new IdNomeDto
            {
                Id = (int?)contaPaga.IdTransportador,
                Nome = contaPaga.NomeTransportador,
            };

            this.DescricaoContaAPagar = contaPaga.DescrContaPagar;
            this.FormaPagamento = contaPaga.DescrFormaPagto;
            this.DescricaoContabil = contaPaga.DescricaoContaContabil;
            this.ValorVencimento = contaPaga.ValorVenc;
            this.ValoresPagamento = new ValoresPagamentoDto
            {
                ValorPago = contaPaga.ValorPago,
                Juros = contaPaga.Juros,
                Multa = contaPaga.Multa,
            };

            this.Parcela = new V1.Lista.ParcelasDto
            {
                Numero = contaPaga.NumParc,
                Total = contaPaga.NumParcMax,
                Exibir = contaPaga.NumParc + contaPaga.NumParcMax > 0 && contaPaga.IdCustoFixo == null,
            };

            this.Datas = new DatasDto
            {
                Vencimento = contaPaga.DataVenc,
                Pagamento = contaPaga.DataPagto,
            };

            this.Observacoes = new ObservacoesDto
            {
                ContaPaga = contaPaga.Obs,
                Desconto = contaPaga.ObsDescAcresc,
                Acrescimo = contaPaga.ObsDescAcresc,
            };

            this.CorLinha = this.ObterCorLinha(contaPaga);
            this.Permissoes = new PermissoesDto
            {
                Editar = contaPaga.EditVisible,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.ContaPagar, contaPaga.IdContaPg, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do plano de contas.
        /// </summary>
        [DataMember]
        [JsonProperty("idPlanoConta")]
        public int? IdPlanoConta { get; set; }

        /// <summary>
        /// Obtém ou define a referência da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor associado a conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o transportador associado a conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("transportador")]
        public IdNomeDto Transportador { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoContaAPagar")]
        public string DescricaoContaAPagar { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public string FormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define a descrição contabil da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoContabil")]
        public string DescricaoContabil { get; set; }

        /// <summary>
        /// Obtém ou define o valor da conta paga até o prazo de vencimento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorVencimento")]
        public decimal? ValorVencimento { get; set; }

        /// <summary>
        /// Obtém ou define os dados de valores associados a conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("valoresPagamento")]
        public ValoresPagamentoDto ValoresPagamento { get; set; }

        /// <summary>
        /// Obtém ou define os dados das parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("parcela")]
        public V1.Lista.ParcelasDto Parcela { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as datas.
        /// </summary>
        [DataMember]
        [JsonProperty("datas")]
        public DatasDto Datas { get; set; }

        /// <summary>
        /// Obtém ou define o destino do pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("destinoPagamento")]
        public string DestinoPagamento { get; set; }

        /// <summary>
        /// Obtém ou define os dados das observações.
        /// </summary>
        [DataMember]
        [JsonProperty("observacoes")]
        public ObservacoesDto Observacoes { get; set; }

        /// <summary>
        /// Obtém ou define a cor referente a linha onde será exibida a conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define as permissões concebidas a conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(Data.Model.ContasPagar contaPaga)
        {
            if (contaPaga.Paga)
            {
                return "Blue";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}