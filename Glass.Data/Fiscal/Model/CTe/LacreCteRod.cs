using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(LacreCteRodDAO))]
    [PersistenceClass("lacre_cte_rod")]
    public class LacreCteRod
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("NUMEROLACRE", PersistenceParameterType.Key)]
        public string NumeroLacre { get; set; }

        #endregion
    }
}
