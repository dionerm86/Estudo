using System;
using GDA;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AcertoDAO))]
    [PersistenceClass("acerto")]
    public class Acerto
    {
        #region Construtores

        public Acerto()
        {
            Situacao = 1;
        }

        public Acerto(uint idCliente)
            : this()
        {
            IdCli = idCliente;
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

        [PersistenceProperty("IDACERTO", PersistenceParameterType.IdentityKey)]
        public uint IdAcerto { get; set; }

        [Log("Taxa de Antecipação")]
        [PersistenceProperty("TAXAANTECIP")]
        public decimal? TaxaAntecip { get; set; }

        [Log("Núm. Aut. Construcard")]
        [PersistenceProperty("NUMAUTCONSTRUCARD")]
        public string NumAutConstrucard { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("ID_CLI")]
        public uint IdCli { get; set; }

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

        [PersistenceProperty("IDSCONTASR")]
        public string IdsContasR { get; set; }

        [PersistenceProperty("VALORESR")]
        public string ValoresR { get; set; }

        [PersistenceProperty("IDSCHEQUESR")]
        public string IdsChequesR { get; set; }

        [PersistenceProperty("TOTALPAGAR")]
        public decimal? TotalPagar { get; set; }

        [PersistenceProperty("TOTALPAGO")]
        public decimal? TotalPago { get; set; }

        [PersistenceProperty("JUROSRECEBIMENTO")]
        public decimal? JurosRecebimento { get; set; }

        [PersistenceProperty("DATARECEBIMENTO")]
        public DateTime? DataRecebimento { get; set; }

        [PersistenceProperty("IDLOJARECEBIMENTO")]
        public int? IdLojaRecebimento { get; set; }

        [PersistenceProperty("DESCONTARCOMISSAO")]
        public bool? DescontarComissao { get; set; }

        [PersistenceProperty("RECEBIMENTOPARCIAL")]
        public bool? RecebimentoParcial { get; set; }

        [PersistenceProperty("RECEBIMENTOCAIXADIARIO")]
        public bool? RecebimentoCaixaDiario { get; set; }

        [PersistenceProperty("RECEBIMENTOGERARCREDITO")]
        public bool? RecebimentoGerarCredito { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("USUCAD")]
        public uint UsuCad { get; set; }

        [Log("Data do Acerto")]
        [PersistenceProperty("DATACAD")]
        public DateTime DataCad { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _nomeCliente;

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente
        {
            get { return _nomeCliente != null ? _nomeCliente.ToUpper() : String.Empty; }
            set { _nomeCliente = value; }
        }

        [PersistenceProperty("TOTALACERTO", DirectionParameter.InputOptional)]
        public decimal TotalAcerto { get; set; }

        [PersistenceProperty("FORMAPAGTO", DirectionParameter.InputOptional)]
        public string FormaPagto { get; set; }

        [PersistenceProperty("FUNCIONARIO", DirectionParameter.InputOptional)]
        public string Funcionario { get; set; }

        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        public string IdPedido { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO", DirectionParameter.InputOptional)]
        public string IdLiberarPedido { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Criterio { get; set; }

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
            get { return IdCli + " - " + _nomeCliente; }
        }

        [Log("Movimentação Crédito")]
        public string MovimentacaoCredito
        {
            get
            {
                if (FinanceiroConfig.FinanceiroRec.EsconderInfoCreditoAcerto)
                    return String.Empty;

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

        [Log("Referência")]
        public string Referencia
        {
            get
            {
                string retorno = "";
                if (!String.IsNullOrEmpty(IdPedido))
                    retorno += "Pedido(s): " + IdPedido + " ";

                if (!String.IsNullOrEmpty(IdLiberarPedido))
                    retorno += "Liberação: " + IdLiberarPedido + " ";

                return retorno.Trim();
            }
        }

        public bool Renegociacao
        {
            get { return ContasReceberDAO.Instance.GetCountRenegByAcerto(IdAcerto, false) > 0; }
        }

        public bool ExibirNotaPromissoria
        {
            get { return Renegociacao && FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria > 0; }
        }

        [PersistenceProperty("Juros", DirectionParameter.InputOptional)]
        public decimal Juros { get; set; }

        /// <summary>
        /// Define se alguma das contas recebidas no acerto está no juridico
        /// </summary>
        public bool PossuiContaJuridico
        {
            get
            {
                //Valida se o acerto possui conta juridico
                return ContasReceberDAO.Instance.AcertoPossuiContasJuridico(null, (int)IdAcerto);
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