using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FixacaoVidroDAO))]
	[PersistenceClass("fixacao_vidro")]
	public class FixacaoVidro : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDFIXACAOVIDRO", PersistenceParameterType.IdentityKey)]
        public int IdFixacaoVidro { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SIGLA")]
        public string Sigla { get; set; }

        #endregion
    }
}