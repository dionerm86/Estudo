using GDA;
using Glass.Data.DAL;
using System;

namespace Glass.Data.Model
{
    [Serializable]
    [PersistenceBaseDAO(typeof(LogMDFeDAO))]
    [PersistenceClass("log_mdfe")]
    public class LogMDFe
    {
        #region Propriedades

        [PersistenceProperty("IDLOGMDFE", PersistenceParameterType.IdentityKey)]
        public int IdLogMDFe { get; set; }

        [PersistenceProperty("IDMANIFESTOELETRONICO")]
        public int IdManifestoEletronico { get; set; }

        [PersistenceProperty("EVENTO")]
        public string Evento { get; set; }

        [PersistenceProperty("CODIGO")]
        public int Codigo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("DATAHORA")]
        public DateTime DataHora { get; set; }

        #endregion
    }
}
