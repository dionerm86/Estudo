using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ObsLancFiscalDAO))]
    [PersistenceClass("obs_lanc_fiscal")]
    public class ObsLancFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDOBSLANCFISCAL", PersistenceParameterType.IdentityKey)]
        public uint IdObsLancFiscal { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }
}