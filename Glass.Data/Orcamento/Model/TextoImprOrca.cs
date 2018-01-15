using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TextoImprOrcaDAO))]
	[PersistenceClass("texto_impr_orca")]
	public class TextoImprOrca
    {
        #region Propriedades

        [PersistenceProperty("IDTEXTOIMPRORCA", PersistenceParameterType.IdentityKey)]
        public uint IdTextoImprOrca { get; set; }

        [PersistenceProperty("TITULO")]
        public string Titulo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("BUSCARSEMPRE")]
        public bool BuscarSempre { get; set; }

        #endregion
    }
}