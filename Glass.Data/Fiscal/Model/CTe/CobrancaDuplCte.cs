using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(CobrancaDuplCteDAO))]
    [PersistenceClass("cobranca_dupl_cte")]
    public class CobrancaDuplCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("NUMERODUPL", PersistenceParameterType.Key)]
        public string NumeroDupl { get; set; }

        [PersistenceProperty("DATAVENC")]
        public DateTime? DataVenc { get; set; }

        [PersistenceProperty("VALORDUPL")]
        public decimal ValorDupl { get; set; }

        #endregion
    }
}
