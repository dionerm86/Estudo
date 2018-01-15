using System;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AcertoChequeDAO))]
    [PersistenceClass("acerto_cheque")]
    public class AcertoCheque
    {
        #region Construtores

        public AcertoCheque()
        {
            Situacao = (int)SituacaoEnum.Aberto;
            DataAcerto = DateTime.Now;

            if (UserInfo.GetUserInfo != null)
                IdFunc = UserInfo.GetUserInfo.CodUser;
        }

        #endregion

        #region Enumeradores

        public enum SituacaoEnum
        {
            Aberto = 1,
            Cancelado
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDACERTOCHEQUE", PersistenceParameterType.IdentityKey)]
        public uint IdAcertoCheque { get; set; }

        [Log("Cliente", "Nome", typeof(ClienteDAO))]
        [PersistenceProperty("IDCLIENTE")]
        public uint? IdCliente { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IDFUNC")]
        public uint? IdFunc { get; set; }

        [Log("Data do Acerto")]
        [PersistenceProperty("DATAACERTO")]
        public DateTime DataAcerto { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALORACERTO")]
        public decimal ValorAcerto { get; set; }

        [Log("Juros")]
        [PersistenceProperty("JUROS")]
        public float Juros { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("VALORCREDITOAOCRIAR")]
        public decimal? ValorCreditoAoCriar { get; set; }

        [PersistenceProperty("CREDITOGERADOCRIAR")]
        public decimal? CreditoGeradoCriar { get; set; }

        [PersistenceProperty("CREDITOUTILIZADOCRIAR")]
        public decimal? CreditoUtilizadoCriar { get; set; }

        [Log("Desconto")]
        [PersistenceProperty("DESCONTO")]
        public decimal Desconto { get; set; }

        [Log("Observação")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("FORMASPAGTO", DirectionParameter.InputOptional)]
        public string FormasPagto { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("CHEQUESPROPRIOS", DirectionParameter.InputOptional)]
        public bool ChequesProprios { get; set; }

        [PersistenceProperty("DATACADcHEQUE", DirectionParameter.InputOptional)]
        public bool DataCadCheque { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string FormaPagto
        {
            get { return FormasPagto; }
        }

        public bool CancelVisible
        {
            get { return Situacao != (int)SituacaoEnum.Cancelado && AcertoChequeDAO.Instance.PodeCancelar(IdAcertoCheque); }
        }

        [Log("Situação")]
        public string DescrSituacao
        {
            get
            {
                switch ((SituacaoEnum)Situacao)
                {
                    case SituacaoEnum.Aberto: return "Aberto";
                    case SituacaoEnum.Cancelado: return "Cancelado";
                    default: return "";
                }
            }
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

        #endregion
    }
}