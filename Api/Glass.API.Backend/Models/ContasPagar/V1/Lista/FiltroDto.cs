// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ContasPagar;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de contas a pagar.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaContasPagar(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da conta a pagar.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da compra.
        /// </summary>
        [JsonProperty("idCompra")]
        public int? IdCompra { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("numeroNotaFiscal")]
        public string NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do custo fixo.
        /// </summary>
        [JsonProperty("idCustoFixo")]
        public int? IdCustoFixo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do imposto/serviço.
        /// </summary>
        [JsonProperty("idImpostoServico")]
        public int? IdImpostoServico { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pagamento restante.
        /// </summary>
        [JsonProperty("idPagamentoRestante")]
        public int? IdPagamentoRestante { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de vencimento das contas a pagar.
        /// </summary>
        [JsonProperty("periodoVencimentoInicio")]
        public DateTime? PeriodoVencimentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de vencimento das contas a pagar.
        /// </summary>
        [JsonProperty("periodoVencimentoFim")]
        public DateTime? PeriodoVencimentoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de cadastro das contas a pagar.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de cadastro das contas a pagar.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das formas de pagamento.
        /// </summary>
        [JsonProperty("idsFormaPagamento")]
        public IEnumerable<uint> IdsFormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial na faixa de valores daS contas a pagar.
        /// </summary>
        [JsonProperty("valorInicial")]
        public decimal? ValorInicial { get; set; }

        /// <summary>
        /// Obtém ou define o valor finaL na faixa de valores daS contas a pagar.
        /// </summary>
        [JsonProperty("valorFinal")]
        public decimal? ValorFinal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscados cheques próprios da empresa junto as contas a pagar.
        /// </summary>
        [JsonProperty("buscarCheques")]
        public bool? BuscarCheques { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da conta a pagar.
        /// </summary>
        [JsonProperty("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será buscada a previsão do custo fixo.
        /// </summary>
        [JsonProperty("buscarPrevisaoCustoFixo")]
        public bool? BuscarPrevisaoCustoFixo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas apenas contas de comissão.
        /// </summary>
        [JsonProperty("apenasContasDeComissao")]
        public bool? ApenasContasDeComissao { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do plano de contas.
        /// </summary>
        [JsonProperty("planoConta")]
        public string PlanoConta { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas apenas as contas de custo fixo.
        /// </summary>
        [JsonProperty("apenasContasDeCustoFixo")]
        public bool? ApenasContasDeCustoFixo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas apenas contas com valor a pagar.
        /// </summary>
        [JsonProperty("apenasContasComValorAPagar")]
        public bool? ApenasContasComValorAPagar { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de pagamento das contas a pagar..
        /// </summary>
        [JsonProperty("periodoPagamentoInicio")]
        public DateTime? PeriodoPagamentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de pagamento das contas a pagar.
        /// </summary>
        [JsonProperty("periodoPagamentoFim")]
        public DateTime? PeriodoPagamentoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de emissão da nota fiscal.
        /// </summary>
        [JsonProperty("periodoNotaFiscalInicio")]
        public DateTime? PeriodoNotaFiscalInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de emissão da nota fiscal.
        /// </summary>
        [JsonProperty("periodoNotaFiscalFim")]
        public DateTime? PeriodoNotaFiscalFim { get; set; }

        /// <summary>
        /// Obtém ou define o número da CTe associada a conta a pagar.
        /// </summary>
        [JsonProperty("numeroCte")]
        public int? NumeroCte { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da transportadora.
        /// </summary>
        [JsonProperty("idTransportadora")]
        public int? IdTransportadora { get; set; }

        /// <summary>
        /// Obtém ou define o nome da transportadora.
        /// </summary>
        [JsonProperty("nomeTransportadora")]
        public string NomeTransportadora { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário da comissão.
        /// </summary>
        [JsonProperty("idFuncionarioComissao")]
        public int? IdFuncionarioComissao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da comissão.
        /// </summary>
        [JsonProperty("idComissao")]
        public int? IdComissao { get; set; }
    }
}