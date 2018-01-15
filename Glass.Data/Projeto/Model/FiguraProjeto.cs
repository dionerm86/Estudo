using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FiguraProjetoDAO))]
    [PersistenceClass("figura_projeto")]
    public class FiguraProjeto
    {
        #region Propriedades

        [PersistenceProperty("IDFIGURAPROJETO", PersistenceParameterType.IdentityKey)]
        public uint IdFiguraProjeto { get; set; }

        [PersistenceProperty("IDGRUPOFIGPROJ")]
        public uint IdGrupoFigProj { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        [PersistenceProperty("CODINTERNO")]
        public string CodInterno { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("DESCRGRUPOFIGURA", DirectionParameter.InputOptional)]
        public string DescrGrupoFigura { get; set; }

        #endregion

        #region Propriedaes de Suporte

        public string DescrSituacao
        {
            get { return Situacao == 1 ? "Ativo" : Situacao == 2 ? "Inativo" : "N/D"; }
        }

        public string FiguralUrl
        {
            get { return Utils.GetFigurasProjetoVirtualPath + IdFiguraProjeto + ".jpg"; }
        }

        #endregion
    }
}