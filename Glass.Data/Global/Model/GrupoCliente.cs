using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(GrupoClienteDAO))]
    [PersistenceClass("grupo_cliente")]
    public class GrupoCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IdGrupoCliente", PersistenceParameterType.IdentityKey)]
        public int IdGrupoCliente { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        #endregion
    }
}
