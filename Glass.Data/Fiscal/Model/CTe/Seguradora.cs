using System;
using GDA;
using Glass.Data.DAL.CTe;

namespace Glass.Data.Model.Cte
{
    [Serializable]
    [PersistenceBaseDAO(typeof(SeguradoraDAO))]
    [PersistenceClass("seguradora")]
    public class Seguradora : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDSEGURADORA", PersistenceParameterType.IdentityKey)]
        public int IdSeguradora { get; set; }

        [PersistenceProperty("NOMESEGURADORA")]
        public string NomeSeguradora { get; set; }

        [PersistenceProperty("CNPJ")]
        public string CNPJ { get; set; }

        #endregion
    }
}
