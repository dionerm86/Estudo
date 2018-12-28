// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Glass.Data.Helper;

namespace Glass.API.Backend.Models.ContasPagar.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das contas a pagar para a tela de listagem.
    /// </summary>
    [DataContract(Name = "ContasPagar")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="contaPagar">A model de contas a pagar.</param>
        internal ListaDto(Data.Model.ContasPagar contaPagar)
        {
            this.Id = (int)contaPagar.IdContaPg;
            this.IdNotaFiscal = (int?)contaPagar.IdNf;
            this.IdComissao = (int?)contaPagar.IdComissao;
            this.IdPlanoConta = (int?)contaPagar.IdConta;
            this.Referencia = contaPagar.Referencia;
            this.Fornecedor = new IdNomeDto
            {
                Id = (int)(contaPagar.IdFornec ?? 0),
                Nome = contaPagar.NomeFornec,
            };

            this.Transportador = new IdNomeDto
            {
                Id = (int)(contaPagar.IdTransportador ?? 0),
                Nome = contaPagar.NomeTransportador,
            };

            this.DescricaoContaAPagar = contaPagar.DescrContaPagar;
            this.ValorVencimento = contaPagar.ValorVenc;
            this.BoletoChegou = contaPagar.BoletoChegou;
            this.ContaContabil = contaPagar.DescricaoContaContabil;
            this.FormaPagamento = new IdNomeDto
            {
                Id = (int)(contaPagar.IdFormaPagto ?? 0),
                Nome = contaPagar.DescrFormaPagto,
            };

            this.Parcela = new ParcelasDto
            {
                Numero = contaPagar.NumParc,
                Total = contaPagar.NumParcMax,
            };

            this.Datas = new DatasDto
            {
                Vencimento = contaPagar.DataVenc,
                Cadastro = contaPagar.DataCad,
            };

            this.Observacoes = new ObservacoesDto
            {
                ContaAPagar = contaPagar.DescrContaPagar,
                Desconto = contaPagar.ObsDescAcresc,
                Acrescimo = contaPagar.ObsDescAcresc,
            };

            this.CorLinha = this.ObterCorLinha(contaPagar);
            this.Permissoes = new PermissoesDto
            {
                Editar = contaPagar.EditVisible,
                EditarDataVencimento = Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.EditarDataVencimentoContaPagar),
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.ContaPagar, contaPagar.IdPagto ?? 0, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("idNotaFiscal")]
        public int? IdNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da comissão.
        /// </summary>
        [DataMember]
        [JsonProperty("idComissao")]
        public int? IdComissao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("idPlanoConta")]
        public int? IdPlanoConta { get; set; }

        /// <summary>
        /// Obtém ou define a referência da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define os dados do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define os dados do transportador.
        /// </summary>
        [DataMember]
        [JsonProperty("transportador")]
        public IdNomeDto Transportador { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoContaAPagar")]
        public string DescricaoContaAPagar { get; set; }

        /// <summary>
        /// Obtém ou define o valor da conta a pagar até o prazo de vencimento.
        /// </summary>
        [DataMember]
        [JsonProperty("valorVencimento")]
        public decimal? ValorVencimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o boleto referente a conta a pagar chegou.
        /// </summary>
        [DataMember]
        [JsonProperty("boletoChegou")]
        public bool? BoletoChegou { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da conta contabil.
        /// </summary>
        [DataMember]
        [JsonProperty("contaContabil")]
        public string ContaContabil { get; set; }

        /// <summary>
        /// Obtém ou define os dados pertinentes a forma de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public IdNomeDto FormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define os dados pertinentes a parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("parcela")]
        public ParcelasDto Parcela { get; set; }

        /// <summary>
        /// Obtém ou define os dados pertinentes as datas.
        /// </summary>
        [DataMember]
        [JsonProperty("datas")]
        public DatasDto Datas { get; set; }

        /// <summary>
        /// Obtém ou define os dados pertinentes as observações.
        /// </summary>
        [DataMember]
        [JsonProperty("observacoes")]
        public ObservacoesDto Observacoes { get; set; }

        /// <summary>
        /// Obtém ou define a cor da fonte na linha em que se encontra a conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define as permissões concebidas a conta a pagar.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(Data.Model.ContasPagar contaPagar)
        {
            if (contaPagar.PrevisaoCustoFixo)
            {
                return "Blue";
            }
            else if (contaPagar.Paga)
            {
                return "MediumSeaGreen";
            }
            else if (contaPagar.PossuiNfDevolucao)
            {
                return "Red";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}