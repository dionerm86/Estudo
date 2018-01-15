using GDA;
using Glass.Data.DAL;
using Glass.Log;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(TipoClienteDAO))]
    [PersistenceClass("tipo_cliente")]
    public class TipoCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDTIPOCLIENTE", PersistenceParameterType.IdentityKey)]
        public int IdTipoCliente { get; set; }

        [Log("Descrição")]
        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        [Log("Cobrar Área Mínima")]
        [PersistenceProperty("COBRARAREAMINIMA")]
        public bool CobrarAreaMinima { get; set; }

        #endregion

        #region Propriedades de Suporte

        public bool DeleteVisible
        {
            get
            {
                //if tipo do funcionario nao for admin, retorna falso, etc
                
                return true;
            }
        }

        #endregion
    }
}