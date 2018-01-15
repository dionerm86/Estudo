using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("codigo_ferragem")]
    public class CodigoFerragem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCODIGOFERRAGEM", PersistenceParameterType.IdentityKey)]
        public int IdCodigoFerragem { get; set; }

        [PersistenceProperty("IDFERRAGEM")]
        [PersistenceForeignKey(typeof(Ferragem), "IdFerragem")]
        public int IdFerragem { get; set; }

        [PersistenceProperty("CODIGO")]
        public string Codigo { get; set; }

        #endregion
    }
}
