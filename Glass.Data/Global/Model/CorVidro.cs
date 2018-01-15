using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CorVidroDAO))]
	[PersistenceClass("cor_vidro")]
	public class CorVidro : Colosoft.Data.BaseModel
    {
        #region Propriedades
        [PersistenceProperty("IDCORVIDRO", PersistenceParameterType.IdentityKey)]
        public int IdCorVidro { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SIGLA")]
        public string Sigla { get; set; }

        #endregion
    }
}