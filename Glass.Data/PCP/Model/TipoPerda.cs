using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoPerdaDAO))]
    [PersistenceClass("tipo_perda")]
    public class TipoPerda : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOPERDA", PersistenceParameterType.IdentityKey)]
        public int IdTipoPerda { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Setor", "Descricao", typeof(SetorDAO))]
        [PersistenceProperty("IDSETOR")]
        [PersistenceForeignKey(typeof(Setor), "IdSetor")]
        public int? IdSetor { get; set; }

        #endregion

        #region Propriedades Estendidas

        private string _descrSetor;

        [PersistenceProperty("DESCRSETOR", DirectionParameter.InputOptional)]
        public string DescrSetor
        {
            get { return IdSetor > 0 ? _descrSetor : "Todos"; }
            set { _descrSetor = value; }
        }

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Administrador; }
        }

        public bool AtribuidoASetor
        {
            get
            {
                return (IdSetor != null && IdSetor > 0) ? true : false;
            }
        }

        public bool MarcarRelatorio { get; set; }

        #endregion
    }
}