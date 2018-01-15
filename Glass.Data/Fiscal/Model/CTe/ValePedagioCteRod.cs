using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ValePedagioCteRodDAO))]
    [PersistenceClass("vale_pedagio_cte_rod")]
    public class ValePedagioCteRod
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDFORNEC", PersistenceParameterType.Key)]
        public uint IdFornec { get; set; }

        [PersistenceProperty("NUMEROCOMPRA", PersistenceParameterType.Key)]
        public string NumeroCompra { get; set; }

        [PersistenceProperty("CNPJCOMPRADOR")]
        public string CnpjComprador { get; set; }

        #endregion
    }
}
