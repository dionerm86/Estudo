﻿using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(SinalDAO))]
    [PersistenceClass("sinal")]
    public class Sinal
    {
        #region Construtores

        public Sinal()
        {
            Situacao = 1;
        }

        public Sinal(uint idCliente)
            : this()
        {
            IdCliente = idCliente;
        }

        #endregion

        #region Enumeradores

        public enum SituacaoEnum
        {
            Aberto = 1,
            Cancelado,
            Processando
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDSINAL", PersistenceParameterType.IdentityKey)]
        public uint IdSinal { get; set; }

        [Log("Núm. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint IdCliente { get; set; }

        [PersistenceProperty("VALORCREDITOAOCRIAR")]
        public decimal? ValorCreditoAoCriar { get; set; }

        [PersistenceProperty("CREDITOGERADOCRIAR")]
        public decimal? CreditoGeradoCriar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCRIAR")]
        public decimal? CreditoUtilizadoCriar { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("IDSPEDIDOSR")]
        public string IdsPedidosR { get; set; }

        [PersistenceProperty("VALORESR")]
        public string ValoresR { get; set; }

        [PersistenceProperty("IDSCHEQUESR")]
        public string IdsChequesR { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        [Log("Data do Sinal")]
        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        [Log("Pagamento Antecipado")]
        [PersistenceProperty("ISPAGTOANTECIPADO")]
        public bool IsPagtoAntecipado { get; set; }

        [PersistenceProperty("DATARECEBIMENTO")]
        public DateTime? DataRecebimento { get; set; }

        [PersistenceProperty("TOTALPAGO")]
        public decimal? TotalPago { get; set; }

        [PersistenceProperty("TOTALPAGAR")]
        public decimal? TotalPagar { get; set; }

        [PersistenceProperty("IDLOJARECEBIMENTO")]
        public int? IdLojaRecebimento { get; set; }

        [PersistenceProperty("DESCONTARCOMISSAO")]
        public bool? DescontarComissao { get; set; }

        [PersistenceProperty("RECEBIMENTOCAIXADIARIO")]
        public bool? RecebimentoCaixaDiario { get; set; }

        [PersistenceProperty("RECEBIMENTOGERARCREDITO")]
        public bool? RecebimentoGerarCredito { get; set; }

        /// <summary>
        /// Saldo devedor ao criar o sinal
        /// </summary>
        [PersistenceProperty("SALDODEVEDOR")]
        public decimal SaldoDevedor { get; set; }

        /// <summary>
        /// Saldo de crédito ao criar o sinal
        /// </summary>
        [PersistenceProperty("SALDOCREDITO")]
        public decimal SaldoCredito { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeCliente;

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
        }

        private decimal _totalSinal;

        [PersistenceProperty("TotalSinal", DirectionParameter.InputOptional)]
        public decimal TotalSinal
        {
            get { return _totalSinal; }
            set { _totalSinal = value; }
        }

        [PersistenceProperty("FORMAPAGTO", DirectionParameter.InputOptional)]
        public string FormaPagto { get; set; }

        [PersistenceProperty("FUNCIONARIO", DirectionParameter.InputOptional)]
        public string Funcionario { get; set; }

        [PersistenceProperty("TelefoneLoja", DirectionParameter.InputOptional)]
        public string TelefoneLoja { get; set; }

        #endregion

        #region Propriedades de Suporte

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)SituacaoEnum.Aberto: return "Aberto";
                    case (int)SituacaoEnum.Cancelado: return "Cancelado";
                    case (int)SituacaoEnum.Processando: return "Processando";
                    default: return string.Empty;
                }
            }
        }

        public string IdNomeCliente
        {
            get { return IdCliente + " - " + _nomeCliente; }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                decimal utilizado = CreditoUtilizadoCriar != null ? CreditoUtilizadoCriar.Value : 0;
                decimal gerado = CreditoGeradoCriar != null ? CreditoGeradoCriar.Value : 0;

                if (ValorCreditoAoCriar == null || (ValorCreditoAoCriar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoCriar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoCriar.Value - utilizado + gerado).ToString("C");
            }
        }

        public bool CancelarVisible
        {
            get { return Situacao != (int)SituacaoEnum.Cancelado; }
        }

        [Log("Retificar Sinal")]
        internal string DadosRetificar { get; set; }

        /// <summary>
        /// Saldo total do cliente. 
        /// Créditos - debítos.
        /// </summary>
        public decimal SaldoTotal
        {
            get
            {
                return SaldoCredito - SaldoDevedor;
            }
        }

        #endregion

        #region Propriedades para Log

        [Log("Cheques")]
        public string ChequesLog { get; set; }

        [Log("Contas")]
        public string ContasLog { get; set; }

        [Log("Formas de Pagamento")]
        public string FormasPagtoLog { get; set; }

        #endregion
    }
}