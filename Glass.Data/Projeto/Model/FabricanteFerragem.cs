using GDA;

namespace Glass.Data.Model
{
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
