using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MedidaItemProjetoDAO))]
	[PersistenceClass("medida_item_projeto")]
	public class MedidaItemProjeto : IMedidaItemProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDMEDIDAITEMPROJETO", PersistenceParameterType.IdentityKey)]
        public uint IdMedidaItemProjeto { get; set; }

        [PersistenceProperty("IDITEMPROJETO")]
        public uint IdItemProjeto { get; set; }

        [PersistenceProperty("IDMEDIDAPROJETO")]
        public uint IdMedidaProjeto { get; set; }

        [PersistenceProperty("VALOR")]
        public int Valor { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEMEDIDAPROJETO", DirectionParameter.InputOptional)]
        public string NomeMedidaProjeto { get; set; }

        #endregion
    }
}