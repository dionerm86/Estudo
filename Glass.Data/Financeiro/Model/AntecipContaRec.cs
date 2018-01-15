using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(AntecipContaRecDAO))]
    [PersistenceClass("antecip_conta_rec")]
    public class AntecipContaRec
    {
        #region Enumeradores

        public enum SituacaoEnum
        {
            Finalizada = 1,
            Cancelada
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDANTECIPCONTAREC", PersistenceParameterType.IdentityKey)]
        public uint IdAntecipContaRec { get; set; }

        [PersistenceProperty("IDFUNCANTECIP")]
        public uint IdFuncAntecip { get; set; }

        [PersistenceProperty("IDCONTABANCO")]
        public uint IdContaBanco { get; set; }

        /// <summary>
        /// 1-Finalizada
        /// 2-Cancelada
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("TAXA")]
        public decimal Taxa { get; set; }

        [PersistenceProperty("JUROS")]
        public decimal Juros { get; set; }

        [PersistenceProperty("IOF")]
        public decimal Iof { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("OBS")]
        public string Obs { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEFUNC", DirectionParameter.InputOptional)]
        public string NomeFunc { get; set; }

        [PersistenceProperty("DESCRCONTABANCO", DirectionParameter.InputOptional)]
        public string DescrContaBanco { get; set; }

        [PersistenceProperty("CRITERIO", DirectionParameter.InputOptional)]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get { return Situacao == 1 ? "Finalizada" : Situacao == 2 ? "Cancelada" : "N/D"; }
        }

        public bool CancelarVisivel
        {
            get { return Situacao == 1; }
        }

        #endregion
    }
}