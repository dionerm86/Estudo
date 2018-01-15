using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("config_menu_tipo_func")]
    public class ConfigMenuTipoFunc : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDCONFIGMENUTIPOFUNC", PersistenceParameterType.IdentityKey)]
        public int IdConfigMenuTipoFunc { get; set; }

        [PersistenceProperty("IDTIPOFUNC")]
        [PersistenceForeignKey(typeof(TipoFuncionario), "IdTipoFuncionario")]
        public int IdTipoFunc { get; set; }

        [PersistenceProperty("IDMENU")]
        [PersistenceForeignKey(typeof(Menu), "IdMenu")]
        public int IdMenu { get; set; }
    }
}
