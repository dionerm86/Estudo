using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FabricanteFerragemDAO))]
    [PersistenceClass("fabricante_ferragem")]
    public class FabricanteFerragem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDFABRICANTEFERRAGEM", PersistenceParameterType.IdentityKey)]
        public int IdFabricanteFerragem { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("SITIO")]
        public string Sitio { get; set; }

        #endregion
    }
}
