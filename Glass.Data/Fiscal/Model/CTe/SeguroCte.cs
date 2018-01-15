using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(SeguroCteDAO))]
    [PersistenceClass("seguro_cte")]
    public class SeguroCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDSEGURADORA")]
        public uint IdSeguradora { get; set; }

        [PersistenceProperty("RESPONSAVELSEGURO")]
        public int ResponsavelSeguro { get; set; }

        [PersistenceProperty("NUMEROAPOLICE")]
        public string NumeroApolice { get; set; }

        [PersistenceProperty("NUMEROAVERBACAO")]
        public string NumeroAverbacao { get; set; }

        [PersistenceProperty("VALORCARGAAVERBACAO")]
        public decimal  ValorCargaAverbacao { get; set; }

        #endregion
    }
}
