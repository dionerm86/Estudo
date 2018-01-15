using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FiguraPecaItemProjetoDAO))]
    [PersistenceClass("figura_peca_item_projeto")]
    public class FiguraPecaItemProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDFIGPECAITEMPROJ", PersistenceParameterType.IdentityKey)]
        public uint IdFigPecaItemProj { get; set; }

        [PersistenceProperty("IDPECAITEMPROJ")]
        public uint IdPecaItemProj { get; set; }

        [PersistenceProperty("IDFIGURAPROJETO")]
        public uint IdFiguraProjeto { get; set; }

        [PersistenceProperty("ITEM")]
        public int Item { get; set; }

        [PersistenceProperty("COORDX")]
        public int CoordX { get; set; }

        [PersistenceProperty("COORDY")]
        public int CoordY { get; set; }

        [PersistenceProperty("TEXTO")]
        public string Texto { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRGRUPOFIGURA", DirectionParameter.InputOptional)]
        public string DescrGrupoFigura { get; set; }

        [PersistenceProperty("CODINTERNOFIGURA", DirectionParameter.InputOptional)]
        public string CodInternoFigura { get; set; }

        #endregion
    }
}