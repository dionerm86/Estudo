using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(LogCteDAO))]
    [PersistenceClass("log_cte")]
    public class LogCte 
    {
        #region Propriedades

        [PersistenceProperty("IDLOGCTE", PersistenceParameterType.IdentityKey)]
        public uint IdLogCte { get; set; }

        [PersistenceProperty("IDCTE")]
        public uint IdCte { get; set; }

        [PersistenceProperty("EVENTO")]
        public string Evento { get; set; }

        [PersistenceProperty("CODIGO")]
        public int Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATAHORA")]
        public DateTime DataHora { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string NumRecibo { get; set; }

        public string NumProtocolo { get; set; }

        #endregion
    }
}
