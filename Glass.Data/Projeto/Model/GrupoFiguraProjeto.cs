using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(GrupoFiguraProjetoDAO))]
    [PersistenceClass("grupo_figura_projeto")]
    public class GrupoFiguraProjeto
    {
        #region Enumeradores

        public enum SituacaoGrupo
        {
            Ativo = 1,
            Inativo
        }

        #endregion

        #region Propriedades

        [PersistenceProperty("IDGRUPOFIGPROJ", PersistenceParameterType.IdentityKey)]
        public uint IdGrupoFigProj { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get { return Situacao == 1 ? "Ativo" : Situacao == 2 ? "Inativo" : "N/D"; }
        }

        #endregion
    }
}