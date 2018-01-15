using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CorFerragemDAO))]
	[PersistenceClass("cor_ferragem")]
	public class CorFerragem : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCORFERRAGEM", PersistenceParameterType.IdentityKey)]
        public int IdCorFerragem { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SIGLA")]
        public string Sigla { get; set; }

        #endregion
    }
}