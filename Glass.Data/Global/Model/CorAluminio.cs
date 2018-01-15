using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(CorAluminioDAO))]
	[PersistenceClass("cor_aluminio")]
	public class CorAluminio : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDCORALUMINIO", PersistenceParameterType.IdentityKey)]
        public int IdCorAluminio { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SIGLA")]
        public string Sigla { get; set; }

        #endregion
    }
}