// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ContasPagar.Pagas;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de contas pagas.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaContasPagas(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da conta paga.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da compra.
        /// </summary>
        [JsonProperty("idCompra")]
        public int? IdCompra { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da comissão.
        /// </summary>
        [JsonProperty("idComissao")]
        public int? IdComissao { get; set; }

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
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("numeroNotaFiscal")]
        public string NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o número da CTe associada a conta paga.
        /// </summary>
        [JsonProperty("numeroCte")]
        public int? NumeroCte { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de cadastro das contas pagas.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de cadastro das contas pagas.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de pagamento das contas pagas.
        /// </summary>
        [JsonProperty("periodoPagamentoInicio")]
        public DateTime? PeriodoPagamentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de pagamento das contas pagas.
        /// </summary>
        [JsonProperty("periodoPagamentoFim")]
        public DateTime? PeriodoPagamentoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial no período de vencimento das contas pagas.
        /// </summary>
        [JsonProperty("periodoVencimentoInicio")]
        public DateTime? PeriodoVencimentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final no período de vencimento das contas pagas.
        /// </summary>
        [JsonProperty("periodoVencimentoFim")]
        public DateTime? PeriodoVencimentoFim { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das formas de pagamento.
        /// </summary>
        [JsonProperty("idsFormaPagamento")]
        public IEnumerable<uint> IdsFormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pagamento restante.
        /// </summary>
        [JsonProperty("idPagamentoRestante")]
        public int? IdPagamentoRestante { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial na faixa de valores daS contas pagas.
        /// </summary>
        [JsonProperty("valorVencimentoInicial")]
        public decimal? ValorVencimentoInicial { get; set; }

        /// <summary>
        /// Obtém ou define o valor finaL na faixa de valores daS contas pagas.
        /// </summary>
        [JsonProperty("valorVencimentoFinal")]
        public decimal? ValorVencimentoFinal { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da conta paga.
        /// </summary>
        [JsonProperty("tipo")]
        public int? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas apenas contas de comissão.
        /// </summary>
        [JsonProperty("apenasContasDeComissao")]
        public bool? ApenasContasDeComissao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas contas pagas que foram renegociadas.
        /// </summary>
        [JsonProperty("buscarRenegociadas")]
        public bool? BuscarRenegociadas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas contas com juros ou multa.
        /// </summary>
        [JsonProperty("buscarContasComJurosMulta")]
        public bool? BuscarContasComJurosMulta { get; set; }

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
        /// Obtém ou define um valor que indica se serão buscadas contas que ainda não foram pagaas.
        /// </summary>
        [JsonProperty("buscarContasPagar")]
        public bool? BuscarContasPagar { get; set; }

        /// <summary>
        /// Obtém ou define a observação da conta paga.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}