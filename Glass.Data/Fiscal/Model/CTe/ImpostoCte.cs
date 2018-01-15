using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ImpostoCteDAO))]
    [PersistenceClass("imposto_cte")]
    public class ImpostoCte : Sync.Fiscal.EFD.Entidade.IImpostoCTe
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("TIPOIMPOSTO", PersistenceParameterType.Key)]
        public int TipoImposto { get; set; }

        [PersistenceProperty("CST")]
        public string Cst { get; set; }

        [PersistenceProperty("BASECALC")]
        public decimal BaseCalc { get; set; }

        [PersistenceProperty("PERCREDBASECALC")]
        public float PercRedBaseCalc { get; set; }

        [PersistenceProperty("ALIQUOTA")]
        public float Aliquota { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("BASECALCSTRETIDO")]
        public decimal BaseCalcStRetido { get; set; }

        [PersistenceProperty("ALIQUOTASTRETIDO")]
        public float AliquotaStRetido { get; set; }

        [PersistenceProperty("VALORSTRETIDO")]
        public decimal ValorStRetido { get; set; }

        [PersistenceProperty("VALORCRED")]
        public decimal ValorCred { get; set; }

        #endregion

        #region IImpostoCTe Members

        Sync.Fiscal.Enumeracao.TipoImposto Sync.Fiscal.EFD.Entidade.IImpostoCTe.TipoImposto
        {
            get { return (Sync.Fiscal.Enumeracao.TipoImposto)TipoImposto; }
        }

        decimal Sync.Fiscal.EFD.Entidade.IImpostoCTe.BaseCalculo
        {
            get { return BaseCalc; }
        }

        string Sync.Fiscal.EFD.Entidade.IImpostoCTe.CST
        {
            get { return Cst; }
        }

        #endregion
    }
}
