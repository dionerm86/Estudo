using System;
using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DepositoChequeDAO))]
    [PersistenceClass("deposito_cheque")]
    public class DepositoCheque
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Aberto = 1,
            Cancelado
        }

        #endregion

        #region Propriedades

        [Log("Id. do Depósito")]
        [PersistenceProperty("IDDEPOSITO", PersistenceParameterType.IdentityKey)]
        public uint IdDeposito { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        [Log("Valor")]
        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [Log("Taxa")]
        [PersistenceProperty("TAXAANTECIP")]
        public decimal TaxaAntecip { get; set; }

        [Log("Data")]
        [PersistenceProperty("DATADEPOSITO")]
        public DateTime DataDeposito { get; set; }

        [PersistenceProperty("USUDEPOSITO")]
        public uint UsuDeposito { get; set; }

        [Log("Obs.")]
        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }
        
        [PersistenceProperty("DADOSCHEQUESDESASSOCIADOSAOCANCELAR")]
        public string DadosChequesDesassociadosAoCancelar { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        [PersistenceProperty("NOMEFUNCDEPOSITO", DirectionParameter.InputOptional)]
        public string NomeFuncDeposito { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        [PersistenceProperty("DADOSCHEQUESANTIGOS", DirectionParameter.InputOptional)]
        public string DadosChequesAntigos { get; set; }

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
                    default: return "";
                }
            }
        }

        public bool BotaoVisible
        {
            get { return Situacao != (int)SituacaoEnum.Cancelado; }
        }

        #endregion
    }
}