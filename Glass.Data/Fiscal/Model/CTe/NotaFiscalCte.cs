using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(NotaFiscalCteDAO))]
    [PersistenceClass("nfe_cte")]
    public class NotaFiscalCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDNF", PersistenceParameterType.Key)]
        public uint IdNf { get; set; }

        #region Dados para relatório

        public string TipoDoc { get; set; }
        public string DocEmitenteNf { get; set; }
        public string Serie { get; set; }
        public string NumeroDoc { get; set; }

        #endregion

        #endregion
    }
}
