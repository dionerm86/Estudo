using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ObsLancFiscalNfDAO))]
    [PersistenceClass("obs_lanc_fiscal_nf")]
    public class ObsLancFiscalNf : Sync.Fiscal.EFD.Entidade.IObservacaoLancamentoFiscal
    {
        #region Propriedades

        [PersistenceProperty("IDNF", PersistenceParameterType.Key)]
        public uint IdNf { get; set; }

        [PersistenceProperty("IDOBSLANCFISCAL", PersistenceParameterType.Key)]
        public uint IdObsLancFiscal { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        #endregion

        #region IObservacaoLancamentoFiscal Members

        int Sync.Fiscal.EFD.Entidade.IObservacaoLancamentoFiscal.Codigo
        {
            get { return (int)IdObsLancFiscal; }
        }

        #endregion
    }
}