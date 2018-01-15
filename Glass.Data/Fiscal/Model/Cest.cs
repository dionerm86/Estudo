using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CestDAO))]
    [PersistenceClass("cest")]
    public class Cest : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCEST", PersistenceParameterType.IdentityKey)]
        public int IdCest { get; set; }

        [PersistenceProperty("Codigo")]
        public string Codigo { get; set; }

        #endregion
    }
}