using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ComplPassagemCteDAO))]
    [PersistenceClass("compl_passagem_cte")]
    public class ComplPassagemCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("NUMSEQPASSAGEM", PersistenceParameterType.Key)]
        public int NumSeqPassagem { get; set; }

        [PersistenceProperty("SIGLAPASSAGEM")]
        public string SiglaPassagem { get; set; }

        #endregion
    }
}
