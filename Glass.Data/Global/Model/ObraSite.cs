using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ObraSiteDAO))]
    [PersistenceClass("obra_site")]
    public class ObraSite
    {
        #region Propriedades

        [PersistenceProperty("CodObra", PersistenceParameterType.IdentityKey)]
        public uint CodObra { get; set; }

        [PersistenceProperty("Descricao")]
        public string Descricao { get; set; }

        [PersistenceProperty("Foto")]
        public string Foto { get; set; }

        #endregion
    }
}