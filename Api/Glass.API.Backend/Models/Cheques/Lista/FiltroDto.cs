// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Cheques;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de cheques.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCheques(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da liberação.
        /// </summary>
        [JsonProperty("idLiberacao")]
        public int? IdLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do acerto.
        /// </summary>
        [JsonProperty("idAcerto")]
        public int? IdAcerto { get; set; }

        /// <summary>
        /// Obtém ou define o número da NF-e.
        /// </summary>
        [JsonProperty("numeroNfe")]
        public int? NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do cheque (1-próprio, 2-terceiros).
        /// </summary>
        [JsonProperty("tipo")]
        public Data.Model.Cheques.TipoCheque? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o número do cheque.
        /// </summary>
        [JsonProperty("numeroCheque")]
        public int? NumeroCheque { get; set; }

        /// <summary>
        /// Obtém ou define a situação do cheque.
        /// </summary>
        [JsonProperty("situacao")]
        public IEnumerable<Data.Model.Cheques.SituacaoCheque> Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cheque foi reapresentado.
        /// </summary>
        [JsonProperty("reapresentado")]
        public bool Reapresentado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cheque está com um advogado, para cobrança/acionamento na justiça.
        /// </summary>
        [JsonProperty("advogado")]
        public Advogado? Advogado { get; set; }

        /// <summary>
        /// Obtém ou define o titular do cheque.
        /// </summary>
        [JsonProperty("titular")]
        public string Titular { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da agência do cheque.
        /// </summary>
        [JsonProperty("agencia")]
        public string Agencia { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da conta do cheque.
        /// </summary>
        [JsonProperty("conta")]
        public string Conta { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial para filtro pelo vencimento do cheque.
        /// </summary>
        [JsonProperty("periodoVencimentoInicio")]
        public DateTime? PeriodoVencimentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final para filtro pelo vencimento do cheque.
        /// </summary>
        [JsonProperty("periodoVencimentoFim")]
        public DateTime? PeriodoVencimentoFim { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial para filtro pela data de cadastro do cheque.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final para filtro pela data de cadastro do cheque.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o CPF/CNPJ do titular do cheque.
        /// </summary>
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente do cheque.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente do cheque.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador fornecedor do cheque.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor do cheque.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o valor inicial do valor do cheque.
        /// </summary>
        [JsonProperty("valorChequeInicio")]
        public decimal ValorChequeInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor final do valor do cheque.
        /// </summary>
        [JsonProperty("valorChequeFim")]
        public decimal ValorChequeFim { get; set; }

        /// <summary>
        /// Obtém ou define o usuário que cadastrou o cheque.
        /// </summary>
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão exibidos apenas cheques recebidos no caixa diário.
        /// </summary>
        [JsonProperty("exibirApenasCaixaDiario")]
        public bool ExibirApenasCaixaDiario { get; set; }

        /// <summary>
        /// Obtém ou define os identificadores das rotas de clientes do cheque.
        /// </summary>
        [JsonProperty("idsRota")]
        public IEnumerable<int> IdsRota { get; set; }

        /// <summary>
        /// Obtém ou define a observação do cheque.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define o filtro que será feito no relatório.
        /// </summary>
        [JsonProperty("ordenacaoFiltro")]
        public int? OrdenacaoFiltro { get; set; }
    }
}
