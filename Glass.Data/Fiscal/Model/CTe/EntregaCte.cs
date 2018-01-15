using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(EntregaCteDAO))]
    [PersistenceClass("entrega_cte")]
    public class EntregaCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("TIPOPERIODODATA")]
        public int TipoPeriodoData { get; set; }

        [PersistenceProperty("TIPOPERIODOHORA")]
        public int TipoPeriodoHora { get; set; }

        [PersistenceProperty("DATAHORAPROG")]
        public DateTime DataHoraProg { get; set; }

        [PersistenceProperty("DATAHORAINI")]
        public DateTime DataHoraIni { get; set; }

        [PersistenceProperty("DATAHORAFIM")]
        public DateTime DataHoraFim { get; set; }

        #endregion
    }
}
