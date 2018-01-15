using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AntecipacaoFornecedorDAO))]
    [PersistenceClass("antecipacao_fornecedor")]
    public class AntecipacaoFornecedor
    {
        #region Enumeradores

        public enum SituacaoAntecipFornec
        {
            Aberta = 1,
            Cancelada,
            Finalizada,
            Confirmada
        }

        public enum TipoPagtoAntecipFornec
        {
            AVista = 1,
            APrazo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDANTECIPFORNEC", PersistenceParameterType.IdentityKey)]
        public uint IdAntecipFornec { get; set; }

        [Log("Fornecedor", "Nomefantasia", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        public uint IdFornec { get; set; }

        [PersistenceProperty("TIPOPAGTO")]
        public int TipoPagto { get; set; }

        [Log("Número Parcelas")]
        [PersistenceProperty("NUMPARCELAS")]
        public int NumParcelas { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [Log("Funcionário Finalização", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCFIN")]
        public uint? IdFuncFin { get; set; }

        /// <summary>
        /// 1 - Aberta
        /// 2 - Cancelada
        /// 3 - Finalizada
        /// 4 - Confirmada
        /// 5 - AguardandoFinanceiro
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        private string _descricao;

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao
        {
            get { return _descricao != null ? _descricao.Replace("\r", "").Replace("\n", "") : String.Empty; }
            set { _descricao = value; }
        }

        [Log("Valor da Antecipacao")]
        [PersistenceProperty("VALORANTECIP")]
        public decimal ValorAntecip { get; set; }

        [Log("Saldo")]
        [PersistenceProperty("SALDO")]
        public decimal Saldo { get; set; }

        [Log("Data da Antecipação")]
        [PersistenceProperty("DATACAD", DirectionParameter.OutputOnlyInsert)]
        public DateTime DataCad { get; set; }

        [Log("Data de Finalização")]
        [PersistenceProperty("DATAFIN")]
        public DateTime? DataFin { get; set; }

        [PersistenceProperty("VALORCREDITOAOCRIAR")]
        public decimal? ValorCreditoAoCriar { get; set; }

        [PersistenceProperty("CREDITOGERADOCRIAR")]
        public decimal? CreditoGeradoCriar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCRIAR")]
        public decimal? CreditoUtilizadoCriar { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("NOMEFUNCFIN", DirectionParameter.InputOptional)]
        public string NomeFuncFin { get; set; }

        [PersistenceProperty("CREDITOFORNEC", DirectionParameter.InputOptional)]
        public decimal CreditoFornec { get; set; }

        [PersistenceProperty("Criterio", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("TotalNotasAbertas", DirectionParameter.InputOptional)]
        public decimal TotalNotasAbertas { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string IdNomeFornec
        {
            get { return IdFornec + " - " + NomeFornec; }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Aberta: return "Aberta";
                    case (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Cancelada: return "Cancelada";
                    case (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Finalizada: return "Finalizada";
                    case (int)AntecipacaoFornecedor.SituacaoAntecipFornec.Confirmada: return "Confirmada";
                    default: return "";
                }
            }
        }

        #region Parcelas

        public DateTime DataPrimParc { get; set; }

        public DateTime DataSegParc { get; set; }

        public DateTime DataTerParc { get; set; }

        public DateTime DataQuarParc { get; set; }

        public DateTime DataQuinParc { get; set; }

        public DateTime[] DatasParcelas
        {
            get
            {
                DateTime[] retorno = new DateTime[5];
                retorno[0] = DataPrimParc;
                retorno[1] = DataSegParc;
                retorno[2] = DataTerParc;
                retorno[3] = DataQuarParc;
                retorno[4] = DataQuinParc;

                return retorno;
            }
            set
            {
                DataPrimParc = value[0];
                DataSegParc = value[1];
                DataTerParc = value[2];
                DataQuarParc = value[3];
                DataQuinParc = value[4];
            }
        }

        public decimal ValorPrimParc { get; set; }

        public decimal ValorSegParc { get; set; }

        public decimal ValorTerParc { get; set; }

        public decimal ValorQuarParc { get; set; }

        public decimal ValorQuinParc { get; set; }

        public decimal[] ValoresParcelas
        {
            get
            {
                decimal[] retorno = new decimal[5];
                retorno[0] = ValorPrimParc;
                retorno[1] = ValorSegParc;
                retorno[2] = ValorTerParc;
                retorno[3] = ValorQuarParc;
                retorno[4] = ValorQuinParc;

                return retorno;
            }
            set
            {
                ValorPrimParc = value[0];
                ValorSegParc = value[1];
                ValorTerParc = value[2];
                ValorQuarParc = value[3];
                ValorQuinParc = value[4];
            }
        }

        #endregion

        #region Formas de pagamento

        public decimal[] ValoresPagto { get; set; }

        public uint[] FormasPagto { get; set; }

        public uint[] TiposCartaoPagto { get; set; }

        public uint[] ParcelasCartaoPagto { get; set; }

        public uint[] ContasBancoPagto { get; set; }

        public string ChequesPagto { get; set; }

        public decimal CreditoUtilizado { get; set; }

        public DateTime? DataRecebimento { get; set; }

        public string[] NumAutCartao { get; set; }

        #endregion

        private bool AntecipFinanceiro
        {
            get
            {
                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
            }
        }

        private bool AntecipFunc
        {
            get { return AntecipFinanceiro || IdFunc == UserInfo.GetUserInfo.CodUser; }
        }

        public bool EditVisible
        {
            get { return Situacao == (int)SituacaoAntecipFornec.Aberta && AntecipFunc; }
        }

        public bool CancelVisible
        {
            get { return Situacao != (int)SituacaoAntecipFornec.Cancelada && AntecipFunc; }
        }

        public bool FinalizarVisible
        {
            get { return Situacao == (int)SituacaoAntecipFornec.Confirmada && AntecipFinanceiro; }
        }

        public bool ReabrirVisible
        {
            get { return AntecipFunc; }
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

        public string DescrFormaPagto
        {
            get
            {
                string retorno = "";
                foreach (PagtoAntecipacaoFornecedor paf in PagtoAntecipacaoFornecedorDAO.Instance.GetByAntecipFornec(IdAntecipFornec))
                    retorno += (string.IsNullOrEmpty(paf.DescrFormaPagto) ? "Crédito" : paf.DescrFormaPagto) + ": " + paf.ValorPagto.ToString("C") + ", ";

                return retorno.TrimEnd(',', ' ');
            }
        }

        public string DescrSaldoNotas
        {
            get
            {
                string retorno = "Saldo da Antecipação: {0:c}\nSaldo Notas Abertas: {1:c}\nSaldo Atual: {2:c}";
                return String.Format(retorno, Saldo, TotalNotasAbertas, Saldo - TotalNotasAbertas);
            }
        }

        public string DescricaoControleFormaPagto
        {
            get { return Descricao + ", Saldo: " + Saldo.ToString("C"); }
        }

        #endregion
    }
}
