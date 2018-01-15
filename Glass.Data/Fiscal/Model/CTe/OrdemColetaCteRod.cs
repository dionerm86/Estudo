using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(OrdemColetaCteRodDAO))]
    [PersistenceClass("ordem_coleta_cte_rod")]
    public class OrdemColetaCteRod
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDTRANSPORTADOR", PersistenceParameterType.Key)]
        public uint IdTransportador { get; set; }

        [PersistenceProperty("NUMERO", PersistenceParameterType.Key)]
        public int Numero { get; set; }

        [PersistenceProperty("SERIE")]
        public string Serie { get; set; }

        [PersistenceProperty("DATAEMISSAO")]
        public DateTime ?DataEmissao { get; set; }

        #endregion
    }
}
