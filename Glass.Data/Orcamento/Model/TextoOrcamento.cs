using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TextoOrcamentoDAO))]
	[PersistenceClass("texto_orcamento")]
	public class TextoOrcamento
    {
        #region Propriedades

        [PersistenceProperty("IDTEXTOORCAMENTO", PersistenceParameterType.IdentityKey)]
        public uint IdTextoOrcamento { get; set; }

        [PersistenceProperty("IDTEXTOIMPRORCA")]
        public uint IdTextoImprOrca { get; set; }

        [PersistenceProperty("IDORCAMENTO")]
        public uint IdOrcamento { get; set; }

        [PersistenceProperty("TITULO", DirectionParameter.InputOptional)]
        public string Titulo { get; set; }

        [PersistenceProperty("DESCRICAO", DirectionParameter.InputOptional)]
        public string Descricao { get; set; }

        #endregion
    }
}