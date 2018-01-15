using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(InfoCteDAO))]
    [PersistenceClass("info_cte")]
    public class InfoCte : Sync.Fiscal.EFD.Entidade.IInfoCTe
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("PRODUTOPREDOMINANTE")]
        public string ProdutoPredominante { get; set; }

        [PersistenceProperty("VALORCARGA")]
        public decimal ValorCarga { get; set; }

        [PersistenceProperty("OUTRASCARACT")]
        public string OutrasCaract { get; set; }

        #endregion

        #region IInfoCTe Members

        string Sync.Fiscal.EFD.Entidade.IInfoCTe.ProdutoPredominante
        {
            get { return ProdutoPredominante; }
        }

        #endregion
    }
}
