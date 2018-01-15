using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(ComplCteDAO))]
    [PersistenceClass("compl_cte")]
    public class ComplCte
    {
        #region Propriedades

        [PersistenceProperty("IDCTE", PersistenceParameterType.Key)]
        public uint IdCte { get; set; }

        [PersistenceProperty("IDROTA")]
        public uint IdRota { get; set; }

        [PersistenceProperty("CARACTTRANSPORTE")]
        public string CaractTransporte { get; set; }

        [PersistenceProperty("CARACTSERVICO")]
        public string CaractServico { get; set; }

        [PersistenceProperty("SIGLAORIGEM")]
        public string SiglaOrigem { get; set; }

        [PersistenceProperty("SIGLADESTINO")]
        public string SiglaDestino { get; set; }

        #endregion
    }
}
