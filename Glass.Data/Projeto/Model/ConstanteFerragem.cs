using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ConstanteFerragemDAO))]
    [PersistenceClass("constante_ferragem")]
    public class ConstanteFerragem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCONSTANTEFERRAGEM", PersistenceParameterType.IdentityKey)]
        public int IdConstanteFerragem { get; set; }

        [PersistenceProperty("IDFERRAGEM")]
        [PersistenceForeignKey(typeof(Ferragem), "IdFerragem")]
        public int IdFerragem { get; set; }

        [PersistenceProperty("NOME")]
        public string Nome { get; set; }

        [PersistenceProperty("VALOR")]
        public double Valor { get; set; }

        #endregion
    }
}
