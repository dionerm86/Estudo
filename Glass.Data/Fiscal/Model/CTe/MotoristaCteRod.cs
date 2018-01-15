using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(MotoristaCteRodDAO))]
    [PersistenceClass("motorista_cte_rod")]
    public class MotoristaCteRod
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDFUNC", PersistenceParameterType.Key)]
        public uint IdFunc { get; set; }       

        #endregion
    }
}
