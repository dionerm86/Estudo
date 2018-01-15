using System;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PagtoDAO))]
	[PersistenceClass("pagto")]
	public class Pagto
    {
        #region Enumeradores

        public enum SituacaoPagto : int
        {
            Finalizado=1,
            Cancelado=2
        }

        public enum FormaPagto : uint
        {
            Dinheiro = 1,
            ChequeProprio,
            Construcard,
            Boleto,
            Cartao,                     // 5
            Permuta,
            Deposito,
            Prazo,
            ChequeTerceiro,
            Credito,                    // 10
            Obra,
            AntecipFornec,
            DepositoNaoIdentificado,
            CartaoNaoIdentificado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDPAGTO", PersistenceParameterType.IdentityKey)]
        public uint IdPagto { get; set; }

		/// <summary>
        /// Este campo não está mais sendo utilizado
        /// </summary>
        [Log("Fornecedor", "Nome", typeof(FornecedorDAO))]
        [PersistenceProperty("IDFORNEC")]
        public uint? IdFornec { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNCPAGTO")]
        public uint IdFuncPagto { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [Log("Data")]
        [PersistenceProperty("DATAPAGTO")]
        public DateTime DataPagto { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALORPAGO")]
        public decimal ValorPago { get; set; }

        [Log("Juros")]
        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [Log("Multa")]
        [PersistenceProperty("MULTA")]
        public decimal Multa { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("MOTIVOCANC")]
        public string MotivoCanc { get; set; }

        [PersistenceProperty("IDSCONTASPG")]
        public string IdsContasPg { get; set; }

        [PersistenceProperty("VALORESPG")]
        public string ValoresPg { get; set; }

        [PersistenceProperty("IDSCHEQUESPG")]
        public string IdsChequesPg { get; set; }

        [PersistenceProperty("RETIFICADO")]
        public bool Retificado { get; set; }

        [PersistenceProperty("VALORCREDITOAOPAGAR")]
        public decimal? ValorCreditoAoPagar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADO")]
        public decimal CreditoUtilizado { get; set; }

        [PersistenceProperty("CREDITOGERADO")]
        public decimal CreditoGerado { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMEFUNCPAGTO", DirectionParameter.InputOptional)]
        public string NomeFuncPagto { get; set; }

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        private string _descrFormaPagto;

        [PersistenceProperty("DESCRFORMAPAGTO", DirectionParameter.InputOptional)]
        public string DescrFormaPagto
        {
            get { return !Renegociacao ? _descrFormaPagto : "Renegociação"; }
            set { _descrFormaPagto = value; }
        }

        [PersistenceProperty("VALORESPAGOS", DirectionParameter.InputOptional)]
        public string ValoresPagos { get; set; }

        [PersistenceProperty("BOLETOSPAGOS", DirectionParameter.InputOptional)]
        public string BoletosPagos { get; set; }
        
        #endregion

        #region Propriedades de suporte

        public string DescrSituacao
        {
            get
            {
                return Situacao == 1 ? "Finalizado":
                    Situacao == 2 ? "Cancelado" :
                    String.Empty;
            }
        }

        public bool CancelarVisible
        {
            get
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                return Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) &&
                    Situacao != (int)SituacaoPagto.Cancelado;
            }
        }

        public string DescrFormaPagtoRpt
        {
            get 
            {
                string[] pagtos = DescrFormaPagto.Split(',');
                string[] valores = !String.IsNullOrEmpty(ValoresPagos) ? ValoresPagos.Split(';') : new string[0];

                string retorno = "";
                for (int i = 0; i < pagtos.Length; i++)
                    retorno += pagtos[i].Trim() + (valores.Length > 0 ? "  " + Glass.Conversoes.StrParaFloat(valores[i]).ToString("C") : "") + ", ";

                return retorno.TrimEnd(' ', ',');
            }
        }

        public string CreditoVisible
        {
            get { return FinanceiroConfig.FormaPagamento.CreditoFornecedor.ToString().ToLower(); }
        }

        public uint[] IdContaPg
        {
            get
            {
                if (String.IsNullOrEmpty(IdsContasPg))
                    return new uint[0];

                string[] ids = IdsContasPg.Split(',');
                return Array.ConvertAll<string, uint>(ids, new Converter<string, uint>(
                    delegate(string id)
                    {
                        uint r;
                        return uint.TryParse(id, out r) ? r : 0;
                    }
                ));
            }
        }

        public decimal[] ValorPagoConta
        {
            get
            {
                if (String.IsNullOrEmpty(ValoresPg))
                    return new decimal[0];

                string[] valores = ValoresPg.Split(',');
                return Array.ConvertAll<string, decimal>(valores, new Converter<string, decimal>(
                    delegate(string valor)
                    {
                        decimal r;
                        return decimal.TryParse(valor.Replace(".", ","), out r) ? r : 0;
                    }
                ));
            }
        }

        public uint[] IdChequePg
        {
            get
            {
                if (String.IsNullOrEmpty(IdsChequesPg))
                    return new uint[0];

                string[] ids = IdsChequesPg.Split(',');
                return Array.ConvertAll<string, uint>(ids, new Converter<string, uint>(
                    delegate(string id)
                    {
                        uint r;
                        return uint.TryParse(id, out r) ? r : 0;
                    }
                ));
            }
        }

        public string MovimentacaoCredito
        {
            get
            {
                decimal utilizado = CreditoUtilizado;
                decimal gerado = CreditoGerado;

                if (ValorCreditoAoPagar == null || (ValorCreditoAoPagar == 0 && (utilizado + gerado) == 0))
                    return "";

                return "Crédito inicial: " + ValorCreditoAoPagar.Value.ToString("C") + "    " +
                    (utilizado > 0 ? "Crédito utilizado: " + utilizado.ToString("C") + "    " : "") +
                    (gerado > 0 ? "Crédito gerado: " + gerado.ToString("C") + "    " : "") +
                    "Saldo de crédito: " + (ValorCreditoAoPagar.Value - utilizado + gerado).ToString("C");
            }
        }

        public bool Renegociacao
        {
            get { return String.IsNullOrEmpty(_descrFormaPagto); }
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