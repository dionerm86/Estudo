using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CavaleteDAO))]
    [PersistenceClass("cavalete")]
    public class Cavalete : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IdCavalete", PersistenceParameterType.IdentityKey)]
        public int IdCavalete { get; set; }

        [Log("Cód. Interno")]
        [PersistenceProperty("CodInterno")]
        public string CodInterno { get; set; }

        [Log("Localização")]
        [PersistenceProperty("Localizacao")]
        public string Localizacao { get; set; }

        #endregion
    }
}
