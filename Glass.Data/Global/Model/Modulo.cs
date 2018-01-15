using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(ModuloDAO))]
    [PersistenceClass("modulo")]
    public class Modulo : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMODULO", PersistenceParameterType.IdentityKey)]
        public int IdModulo { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public string Descricao { get; set; }

        /// <summary>
        /// 1-Ativo
        /// 2-Inativo
        /// </summary>
        [PersistenceProperty("SITUACAO")]
        public Glass.Situacao Situacao { get; set; }

        [PersistenceProperty("GRUPO")]
        public string Grupo { get; set; }

        #endregion
    }
}