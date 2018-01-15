using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("config_menu_func")]
    public class ConfigMenuFunc : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IDCONFIGMENUFUNC", PersistenceParameterType.IdentityKey)]
        public int IdConfigMenuFunc { get; set; }

        [PersistenceProperty("IDMENU")]
        [PersistenceForeignKey(typeof(Menu), "IdMenu")]
        public int IdMenu { get; set; }

        [PersistenceProperty("IDFUNC")]
        [PersistenceForeignKey(typeof(Funcionario), "IdFunc")]
        public int IdFunc { get; set; }
    }
}
