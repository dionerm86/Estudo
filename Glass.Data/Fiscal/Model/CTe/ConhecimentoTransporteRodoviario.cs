using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ConhecimentoTransporteRodoviarioDAO))]
    [PersistenceClass("conhecimento_transporte_rodoviario")]
    public class ConhecimentoTransporteRodoviario
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("DATAPREVISTAENTREGA")]
        public DateTime? DataPrevistaEntrega { get; set; }

        [PersistenceProperty("LOTACAO")]
        public bool Lotacao { get; set; }

        [PersistenceProperty("CIOT")]
        public string CIOT { get; set; }

        #endregion
    }
}
