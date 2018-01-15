using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(PaisDAO))]
    [PersistenceClass("pais")]
    public class Pais : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDPAIS", PersistenceParameterType.IdentityKey)]
        public int IdPais { get; set; }

        [PersistenceProperty("NOMEPAIS")]
        public string NomePais { get; set; }

        [PersistenceProperty("CODPAIS")]
        public string CodPais { get; set; }

        #endregion
    }
}