using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(SubtipoPerdaDAO))]
    [PersistenceClass("subtipo_perda")]
    public class SubtipoPerda : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDSUBTIPOPERDA", PersistenceParameterType.IdentityKey)]
        public int IdSubtipoPerda { get; set; }

        [PersistenceProperty("IDTIPOPERDA")]
        public int IdTipoPerda { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get { return UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Administrador; }
        }

        #endregion
    }
}